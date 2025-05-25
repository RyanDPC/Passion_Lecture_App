using App.Database;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace App.ViewModels
{
    public partial class ReadBookViewModel : ObservableObject
    {
        private int _bookId;
        private readonly BookApi _bookApi = new();

        [ObservableProperty]
        private Book currentBook;

        [ObservableProperty]
        private ObservableCollection<Book> bookList = new();

        [ObservableProperty]
        private int currentPage;

        [ObservableProperty]
        private bool canGoBack;

        [ObservableProperty]
        private bool canGoForward;

        [ObservableProperty]
        private ObservableCollection<Tag> bookTags = new();

        [ObservableProperty]
        private string bookCategory;

        [ObservableProperty]
        private double averageRating;

        [ObservableProperty]
        private ObservableCollection<Chapter> chapters = new();

        [ObservableProperty]
        private Chapter currentChapter;

        [ObservableProperty]
        private int currentChapterIndex;

        public string CurrentChapterTitle => CurrentChapter?.Title ?? "Aucun chapitre";

        public HtmlWebViewSource CurrentChapterHtmlSource => new()
        {
            Html = CurrentChapter?.HtmlContent ?? "<p>Pas de contenu</p>"
        };

        public double ReadingProgress => CurrentBook != null && CurrentBook.Pages > 0
            ? (double)CurrentPage / CurrentBook.Pages
            : 0;

        public string CurrentPageContent => CurrentBook?.Passage ?? "Contenu non disponible";

        public string CurrentPageTitle => $"Chapitre {(CurrentPage / 10) + 1}";

        public bool HasPageTitles => CurrentPage % 10 == 1;

        public bool HasNextChapter => Chapters != null && CurrentChapterIndex < Chapters.Count - 1;

        public string CurrentPageText => $"Page {CurrentPage} sur {CurrentBook?.Pages ?? 1}";

        public ReadBookViewModel(int bookId)
        {
            _bookId = bookId;
        }

        public async Task LoadBookAsync(int bookId)
        {
            _bookId = bookId;
            try
            {
                var books = await _bookApi.GetBookByIdAsync(bookId);
                if (books.Count > 0)
                {
                    CurrentBook = books[0];

                    if (CurrentBook.Chapters != null && CurrentBook.Chapters.Count > 0)
                    {
                        Chapters = new ObservableCollection<Chapter>(CurrentBook.Chapters);
                        CurrentChapterIndex = 0;
                        CurrentChapter = Chapters[CurrentChapterIndex];
                    }

                    if (CurrentBook.Tags != null)
                    {
                        BookTags = new ObservableCollection<Tag>(CurrentBook.Tags);
                    }

                    await SaveBookToLocalDbAsync();
                    await LoadLastPageFromLocalDbAsync();
                }
                else
                {
                    await LoadBookFromLocalDbAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors du chargement du livre depuis l'API: " + ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Détail de l'erreur interne : " + ex.InnerException.Message);
                }

                await LoadBookFromLocalDbAsync();
            }

            UpdateNavigationState();
        }

        private async Task LoadBookFromLocalDbAsync()
        {
            await using var context = new DataContext();
            CurrentBook = await context.Books.FirstOrDefaultAsync(b => b.Id == _bookId);
            if (CurrentBook != null)
            {
                CurrentPage = CurrentBook.LastReadPage > 0 ? CurrentBook.LastReadPage : 1;
            }
        }

        private async Task SaveBookToLocalDbAsync()
        {
            if (CurrentBook == null) return;

            await using var context = new DataContext();
            var localBook = await context.Books.FirstOrDefaultAsync(b => b.Id == CurrentBook.Id);

            if (localBook == null)
            {
                context.Books.Add(CurrentBook);
            }
            else
            {
                localBook.Name = CurrentBook.Name;
                localBook.Passage = CurrentBook.Passage;
                localBook.Summary = CurrentBook.Summary;
                localBook.EditionYear = CurrentBook.EditionYear;
                localBook.CoverImage = CurrentBook.CoverImage;
                localBook.Pages = CurrentBook.Pages;
            }

            await context.SaveChangesAsync();
        }

        private async Task LoadLastPageFromLocalDbAsync()
        {
            await using var context = new DataContext();
            var localBook = await context.Books.FirstOrDefaultAsync(b => b.Id == _bookId);
            if (localBook != null && localBook.LastReadPage > 0)
            {
                CurrentPage = localBook.LastReadPage;
            }
            else
            {
                CurrentPage = 1;
            }
        }

        private void UpdateNavigationState()
        {
            CanGoBack = CurrentPage > 1;
            CanGoForward = CurrentBook != null && CurrentPage < CurrentBook.Pages;
        }

        [RelayCommand]
        public async Task NextPage()
        {
            if (CurrentBook != null && CurrentPage < CurrentBook.Pages)
            {
                CurrentPage++;
                UpdateNavigationState();
                await SaveProgressAsync();
                RefreshPageBindings();
            }
        }

        [RelayCommand]
        public async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateNavigationState();
                await SaveProgressAsync();
                RefreshPageBindings();
            }
        }

        private void RefreshPageBindings()
        {
            OnPropertyChanged(nameof(CurrentPageText));
            OnPropertyChanged(nameof(ReadingProgress));
            OnPropertyChanged(nameof(CurrentPageContent));
            OnPropertyChanged(nameof(CurrentPageTitle));
            OnPropertyChanged(nameof(HasPageTitles));
        }

        private async Task SaveProgressAsync()
        {
            if (CurrentBook == null) return;

            await using var context = new DataContext();
            var bookToUpdate = await context.Books.FirstOrDefaultAsync(b => b.Id == _bookId);
            if (bookToUpdate != null)
            {
                bookToUpdate.LastReadPage = CurrentPage;
            }
            else
            {
                context.Books.Add(new Book
                {
                    Id = _bookId,
                    Name = CurrentBook.Name,
                    LastReadPage = CurrentPage,
                    Pages = CurrentBook.Pages,
                    CoverImage = CurrentBook.CoverImage
                });
            }

            await context.SaveChangesAsync();

            // Synchronisation serveur possible ici
        }

        [RelayCommand]
        public void NextChapter()
        {
            if (Chapters != null && CurrentChapterIndex < Chapters.Count - 1)
            {
                CurrentChapterIndex++;
                CurrentChapter = Chapters[CurrentChapterIndex];
                OnPropertyChanged(nameof(CurrentChapterTitle));
                OnPropertyChanged(nameof(CurrentChapterHtmlSource));
            }
        }

        [RelayCommand]
        public void PreviousChapter()
        {
            if (Chapters != null && CurrentChapterIndex > 0)
            {
                CurrentChapterIndex--;
                CurrentChapter = Chapters[CurrentChapterIndex];
                OnPropertyChanged(nameof(CurrentChapterTitle));
                OnPropertyChanged(nameof(CurrentChapterHtmlSource));
            }
        }
    }
}
