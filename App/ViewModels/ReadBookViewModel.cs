using App.Database;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace App.ViewModels
{
    public partial class ReadBookViewModel : ObservableObject
    {
        private int _bookId;
        private readonly BookApi _bookApi = new();

        [ObservableProperty]
        private Book book;

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

        // Utilisé pour l'affichage du texte de pagination
        public string CurrentPageText => $"Page {CurrentPage} sur {Book?.Pages ?? 1}";

        // Progression de lecture en pourcentage
        public double ReadingProgress => Book != null && Book.Pages > 0 ? (double)CurrentPage / Book.Pages : 0;

        // Contenu de la page courante (dans un vrai livre, ce serait du texte spécifique à la page)
        public string CurrentPageContent => Book?.Passage ?? "Contenu non disponible";

        // Titre de la page courante (simulé)
        public string CurrentPageTitle => $"Chapitre {(CurrentPage / 10) + 1}";
        
        // Indique si le livre a des titres de chapitres
        public bool HasPageTitles => CurrentPage % 10 == 1; // Simuler un titre toutes les 10 pages

        // Constructeur avec un paramètre bookId
        public ReadBookViewModel(int bookId)
        {
            _bookId = bookId;
            LoadBookAsync(bookId).Wait();
        }

        // Charger le livre et la page en cours
        public async Task LoadBookAsync(int bookId)
        {
            // Essayer d'abord de charger depuis l'API (pour avoir toutes les données fraîches)
            try
            {
                var books = await _bookApi.GetBookByIdAsync(bookId);
                if (books.Count > 0)
                {
                    Book = books[0];
                    
                    // Charger les tags, commentaires et autres données associées
                    if (Book.Tags != null)
                    {
                        BookTags = new ObservableCollection<Tag>(Book.Tags);
                    }
                    
                    // Une fois les données récupérées, sauvegarder dans la base de données locale pour accès hors ligne
                    await SaveBookToLocalDbAsync();
                    
                    // Si on a un livre, charger la dernière page lue depuis la base de données locale
                    await LoadLastPageFromLocalDbAsync();
                }
                else
                {
                    // Si le livre n'est pas disponible via l'API, essayer depuis la base de données locale
                    await LoadBookFromLocalDbAsync();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du livre depuis l'API: {ex.Message}");
                // En cas d'erreur, charger depuis la base de données locale
                await LoadBookFromLocalDbAsync();
            }
            
            // Mettre à jour l'état de navigation
            UpdateNavigationState();
        }
        
        private async Task LoadBookFromLocalDbAsync()
        {
            await using var context = new DataContext();
            Book = await context.Books.FirstOrDefaultAsync(b => b.Id == _bookId);
            
            // Si on a un livre, charger la dernière page lue
            if (Book != null)
            {
                CurrentPage = Book.LastReadPage > 0 ? Book.LastReadPage : 1;
            }
        }
        
        private async Task SaveBookToLocalDbAsync()
        {
            if (Book == null) return;
            
            await using var context = new DataContext();
            var localBook = await context.Books.FirstOrDefaultAsync(b => b.Id == Book.Id);
            
            if (localBook == null)
            {
                // Si le livre n'existe pas localement, l'ajouter
                context.Books.Add(Book);
            }
            else
            {
                // Sinon mettre à jour les données du livre local
                localBook.Name = Book.Name;
                localBook.Passage = Book.Passage;
                localBook.Summary = Book.Summary;
                localBook.EditionYear = Book.EditionYear;
                localBook.CoverImage = Book.CoverImage;
                localBook.Pages = Book.Pages;
                // Ne pas écraser LastReadPage car on veut conserver la progression locale
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

        // Mettre à jour l'état des boutons de navigation
        private void UpdateNavigationState()
        {
            CanGoBack = CurrentPage > 1;
            CanGoForward = Book != null && CurrentPage < Book.Pages;
        }

        // Passer à la page suivante
        [RelayCommand]
        public async Task NextPage()
        {
            if (Book != null && CurrentPage < Book.Pages)
            {
                CurrentPage++;
                UpdateNavigationState();
                await SaveProgressAsync();
                OnPropertyChanged(nameof(CurrentPageText));
                OnPropertyChanged(nameof(ReadingProgress));
                OnPropertyChanged(nameof(CurrentPageContent));
                OnPropertyChanged(nameof(CurrentPageTitle));
                OnPropertyChanged(nameof(HasPageTitles));
            }
        }

        // Revenir à la page précédente
        [RelayCommand]
        public async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateNavigationState();
                await SaveProgressAsync();
                OnPropertyChanged(nameof(CurrentPageText));
                OnPropertyChanged(nameof(ReadingProgress));
                OnPropertyChanged(nameof(CurrentPageContent));
                OnPropertyChanged(nameof(CurrentPageTitle));
                OnPropertyChanged(nameof(HasPageTitles));
            }
        }

        // Sauvegarder la progression à la fois localement et sur le serveur
        private async Task SaveProgressAsync()
        {
            if (Book == null) return;
            
            // Mise à jour locale
            await using var context = new DataContext();
            var bookToUpdate = await context.Books.FirstOrDefaultAsync(b => b.Id == _bookId);
            if (bookToUpdate != null)
            {
                bookToUpdate.LastReadPage = CurrentPage;
                await context.SaveChangesAsync();
            }
            else
            {
                // Si le livre n'existe pas dans la base de données locale, l'ajouter
                context.Books.Add(new Book
                {
                    Id = _bookId,
                    Name = Book.Name,
                    LastReadPage = CurrentPage,
                    Pages = Book.Pages,
                    CoverImage = Book.CoverImage
                });
                await context.SaveChangesAsync();
            }
            
            // On pourrait aussi synchroniser avec le serveur
            try
            {
                // Créer un objet avec juste la dernière page lue
                var bookProgress = new
                {
                    id = _bookId,
                    lastReadPage = CurrentPage
                };
                
                // On pourrait envoyer cela au serveur si besoin
                // await _bookApi.UpdateBookProgressAsync(bookProgress);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Erreur lors de la mise à jour de la progression sur le serveur : {ex.Message}");
                // La mise à jour locale a fonctionné, donc on peut continuer
            }
        }
    }
}
