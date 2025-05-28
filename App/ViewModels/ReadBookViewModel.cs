using App.Database;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VersOne.Epub;
using VersOne.Epub.Schema;

namespace App.ViewModels
{
    public partial class ReadBookViewModel : BaseViewModel
    {
        private int _bookId;
        private EpubBook _epubBook;
        private readonly ReadingProgressService _progressService;

        [ObservableProperty]
        private Book currentBook;

        [ObservableProperty]
        private ObservableCollection<EpubLocalTextContentFile> chapters = new();

        [ObservableProperty]
        private int currentChapterIndex;

        [ObservableProperty]
        private string currentChapterTitle;

        [ObservableProperty]
        private HtmlWebViewSource currentHtmlContent = new();

        [ObservableProperty]
        private string currentChapter;

        [ObservableProperty]
        private int currentPage;

        public ReadBookViewModel()
        {
            _progressService = new ReadingProgressService();
        }

        public ReadBookViewModel(int bookId)
        {
            _bookId = bookId;
            _progressService = new ReadingProgressService();
        }

        public int BookId
        {
            get => _bookId;
            set => _bookId = value;
        }

        public async Task LoadBookAsync()
        {
            try 
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Début du chargement du livre {_bookId}");
                
                // Utiliser directement le contexte au lieu de l'API
                await using var context = new DataContext();
                CurrentBook = await context.Books
                    .Include(b => b.Tags)  // Inclure les tags
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == _bookId);
                
                if (CurrentBook != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Livre chargé: {CurrentBook.Name}");
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Taille du contenu: {(CurrentBook.Content?.Length ?? 0)} bytes");
                    if (CurrentBook.Content == null)
                    {
                        // Si le contenu est null, essayer de synchroniser avec l'API
                        await context.SyncBookAsync();
                        CurrentBook = await context.Books
                            .AsNoTracking()
                            .FirstOrDefaultAsync(b => b.Id == _bookId);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] Le livre n'a pas été trouvé!");
                    // Si le livre n'existe pas localement, synchroniser avec l'API
                    await context.SyncBookAsync();
                    CurrentBook = await context.Books
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == _bookId);
                }

                if (CurrentBook?.Content?.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Premier octets EPUB: {BitConverter.ToString(CurrentBook.Content.Take(10).ToArray())}");

                    // Charger le EPUB et son contenu en mémoire
                    using var ms = new MemoryStream(CurrentBook.Content);
                    _epubBook = await EpubReader.ReadBookAsync(ms);

                    // ReadingOrder est une List<EpubLocalTextContentFile>
                    Chapters = new ObservableCollection<EpubLocalTextContentFile>(_epubBook.ReadingOrder);

                    // Afficher le premier chapitre
                    if (Chapters.Any())
                    {
                        SetChapter(0);
                    }
                }
                else
                {
                    Console.WriteLine("Le contenu EPUB est vide ou null !");
                    CurrentHtmlContent = new HtmlWebViewSource { Html = "<p>Contenu indisponible</p>" };
                    CurrentChapterTitle = "Aucun contenu";
                }

                // Charger la progression
                var progress = await _progressService.GetProgressAsync(_bookId);
                if (progress != null)
                {
                    CurrentPage = progress.Page;
                    CurrentChapter = progress.Chapter;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Erreur lors du chargement: {ex.Message}");
                throw;
            }
        }

        private void SetChapter(int index)
        {
            if (index < 0 || index >= Chapters.Count)
                return;

            CurrentChapterIndex = index;
            EpubLocalTextContentFile chapter = Chapters[index];

            // Le nom de fichier est dans la propriété Key
            CurrentChapterTitle = Path.GetFileNameWithoutExtension(chapter.Key);

            // Inject CSS directly into HTML content
            var htmlContent = chapter.Content ?? "<p>(Chapitre vide)</p>";
            var styledHtml = $@"
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'>
                    <style>
                        body {{ 
                            font-family: system-ui; 
                            line-height: 1.6;
                            padding: 20px;
                            max-width: 800px;
                            margin: 0 auto;
                        }}
                        img {{ max-width: 100%; height: auto; }}
                    </style>
                </head>
                <body>
                    {htmlContent}
                </body>
                </html>";

            System.Diagnostics.Debug.WriteLine($"[DEBUG] HTML injecté dans WebView:\n{styledHtml.Substring(0, Math.Min(500, styledHtml.Length))}");

            CurrentHtmlContent = new HtmlWebViewSource { Html = styledHtml };
        }

        [RelayCommand]
        public void NextChapter()
        {
            if (CurrentChapterIndex < Chapters.Count - 1)
            {
                SetChapter(CurrentChapterIndex + 1);
            }
        }

        [RelayCommand]
        public void PreviousChapter()
        {
            if (CurrentChapterIndex > 0)
            {
                SetChapter(CurrentChapterIndex - 1);
            }
        }

        public async Task LoadChaptersFromEpubAsync(VersOne.Epub.EpubBook epubBook)
        {
            Chapters = new ObservableCollection<EpubLocalTextContentFile>(epubBook?.ReadingOrder ?? new List<EpubLocalTextContentFile>());
            
            // Restaurer la dernière position
            var progress = await _progressService.GetProgressAsync(_bookId);
            if (progress != null)
            {
                // Trouver l'index du dernier chapitre lu
                var chapterIndex = Chapters.ToList().FindIndex(c => 
                    Path.GetFileNameWithoutExtension(c.Key) == progress.Chapter);
                
                if (chapterIndex >= 0)
                {
                    SetChapter(chapterIndex);
                    CurrentPage = progress.Page;
                }
            }
            else
            {
                SetChapter(0); // Premier chapitre par défaut
            }
        }

        public async Task SaveReadingProgressAsync()
        {
            if (CurrentChapterIndex >= 0 && CurrentChapterIndex < Chapters.Count)
            {
                var currentChapterName = Path.GetFileNameWithoutExtension(Chapters[CurrentChapterIndex].Key);
                await _progressService.SaveProgressAsync(
                    _bookId,
                    CurrentPage,
                    currentChapterName,
                    0 // plus de scroll position
                );
            }
        }
    }
}
