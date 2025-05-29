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
                await using var context = new DataContext();
                CurrentBook = await context.Books
                    .Include(b => b.Tags)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == _bookId);

                if (CurrentBook != null && CurrentBook.Content == null)
                {
                    await context.SyncBookAsync();
                    CurrentBook = await context.Books
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == _bookId);
                }
                else if (CurrentBook == null)
                {
                    await context.SyncBookAsync();
                    CurrentBook = await context.Books
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == _bookId);
                }

                if (CurrentBook?.Content?.Length > 0)
                {
                    using var ms = new MemoryStream(CurrentBook.Content);
                    _epubBook = await EpubReader.ReadBookAsync(ms);
                    Chapters = new ObservableCollection<EpubLocalTextContentFile>(_epubBook.ReadingOrder);

                    if (Chapters.Any())
                    {
                        SetChapter(0);
                    }
                }
                else
                {
                    CurrentHtmlContent = new HtmlWebViewSource { Html = "<p>Contenu indisponible</p>" };
                    CurrentChapterTitle = "Aucun contenu";
                }

                var progress = await _progressService.GetProgressAsync(_bookId);
                if (progress != null)
                {
                    CurrentPage = progress.Page;
                    CurrentChapter = progress.Chapter;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetChapter(int index)
        {
            if (index < 0 || index >= Chapters.Count)
                return;

            CurrentChapterIndex = index;
            EpubLocalTextContentFile chapter = Chapters[index];
            CurrentChapterTitle = Path.GetFileNameWithoutExtension(chapter.Key);

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

            var progress = await _progressService.GetProgressAsync(_bookId);
            if (progress != null)
            {
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
                SetChapter(0);
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
                    0
                );
            }
        }
    }
}
