using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Book> books = new();

        [ObservableProperty]
        private ObservableCollection<Book> filteredBooks = new();

        [ObservableProperty]
        private ObservableCollection<Tag> availableTags = new();

        [ObservableProperty] private string name;
        [ObservableProperty] private string summary;
        [ObservableProperty] private string passage;
        [ObservableProperty] private string editionYear;
        [ObservableProperty] private ImageSource coverImage;
        [ObservableProperty] private string searchText;

        private byte[] imageBytes;

        private readonly BookApi _bookApi = new();
        private readonly SearchApi _searchApi = new();
        private readonly TagApi _tagApi = new();
        private readonly BookTagApi _bookTagApi = new();

        public BookViewModel()
        {
            LoadBooksAsync();
            LoadTagsAsync();
            AddMockBookWithChapters(); // Ajout du mock pour test
        }

        private void AddMockBookWithChapters()
        {
            var mockBook = new Book
            {
                Id = 999,
                Name = "Livre de Test (Mock)",
                Summary = "Résumé du livre de test.",
                Passage = "Contenu principal du livre de test.",
                EditionYear = 2024,
                Pages = 20,
                Chapters = new List<Chapter>
                {
                    new Chapter
                    {
                        Id = 1,
                        Title = "Chapitre 1 : Introduction",
                        HtmlContent = "<h1>Chapitre 1</h1><p>Contenu du chapitre 1...</p>",
                        BookId = 999
                    },
                    new Chapter
                    {
                        Id = 2,
                        Title = "Chapitre 2 : Développement",
                        HtmlContent = "<h1>Chapitre 2</h1><p>Contenu du chapitre 2...</p>",
                        BookId = 999
                    },
                    new Chapter
                    {
                        Id = 3,
                        Title = "Chapitre 3 : Conclusion",
                        HtmlContent = "<h1>Chapitre 3</h1><p>Contenu du chapitre 3...</p>",
                        BookId = 999
                    }
                }
            };

            Books.Add(mockBook);
            UpdateFilteredBooks();
        }

        private async Task LoadTagsAsync()
        {
            try
            {
                var tags = await _tagApi.GetAllTagsAsync();
                AvailableTags.Clear();
                foreach (var tag in tags)
                {
                    AvailableTags.Add(tag);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des tags : {ex.Message}");
            }
        }

        // Charger les livres depuis l'API
        private async Task LoadBooksAsync()
        {
            try
            {
                var result = await _bookApi.GetAllBooksAsync();
                Books.Clear();
                if (result.Count == 0)
                {
                    Console.WriteLine("Aucun livre trouvé.");
                }

                foreach (var book in result)
                {
                    Books.Add(book);
                    Debug.WriteLine($"Livre ajouté : {book.Name}");
                }
                Console.WriteLine($"Total des livres chargés : {Books.Count}");
                UpdateFilteredBooks();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des livres: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ToggleTag(Tag tag)
        {
            if (tag != null)
            {
                tag.IsSelected = !tag.IsSelected;
                UpdateFilteredBooks();
            }
        }

        private void UpdateFilteredBooks()
        {
            var selectedTags = AvailableTags.Where(t => t.IsSelected).ToList();
            
            if (selectedTags.Count == 0)
            {
                FilteredBooks = new ObservableCollection<Book>(Books);
            }
            else
            {
                var filtered = Books.Where(book => 
                    book.Tags != null && 
                    book.Tags.Any(tag => selectedTags.Contains(tag)))
                    .ToList();
                FilteredBooks = new ObservableCollection<Book>(filtered);
            }
        }

        [RelayCommand]
        private async Task SearchBookAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadBooksAsync();
                    return;
                }

                var url = $"api/search?query={SearchText}&searchType=book";
                Console.WriteLine($"URL de la requête : {url}");

                var result = await _searchApi.SearchBook(SearchText);

                Console.WriteLine($"Résultat de la recherche : {result.Count} livres trouvés.");

                Books.Clear();

                foreach (var book in result)
                {
                    Books.Add(book);
                    Console.WriteLine($"Livre trouvé : {book.Name}");
                }
                UpdateFilteredBooks();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la recherche de livres : {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task AddBookAsync()
        {
            try
            {
                await PickImageAsync();

                var newBook = new Book
                {
                    Name = Name,
                    Summary = Summary,
                    Passage = Passage,
                    EditionYear = int.Parse(EditionYear),
                    CoverImage = imageBytes,
                };

                await _bookApi.CreateBookAsync(newBook);

                Books.Add(newBook);
                UpdateFilteredBooks();

                // Réinitialiser les champs
                Name = "";
                Summary = "";
                Passage = "";
                EditionYear = "";
                CoverImage = null;

                Console.WriteLine($"Livre '{newBook.Name}' ajouté avec succès.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du livre : {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task PickImageAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Choisir une image de couverture",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();

                    // Convertir le flux en byte[] pour un usage ultérieur (par exemple, sauvegarde dans une DB)
                    imageBytes = ConvertStreamToByteArray(stream);

                    // Convertir le flux en ImageSource pour l'affichage
                    CoverImage = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sélection de l'image : {ex.Message}");
            }
        }

        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [RelayCommand]
        private async Task GoToReadBookAsync(int bookId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Navigation demandée pour le livre ID : {bookId}");
                await Shell.Current.GoToAsync($"///readbook?bookId={bookId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Échec navigation vers ReadBookPage : {ex.Message}");
            }
        }


        [RelayCommand]
        private async Task AddTagToBookAsync(Book book)
        {
            try
            {
                if (book == null) return;
                
                // Afficher une page de dialogue pour sélectionner un tag
                var selectedTags = await App.Current.MainPage.DisplayActionSheet(
                    "Sélectionner un tag", 
                    "Annuler", 
                    null, 
                    AvailableTags.Select(t => t.Name).ToArray());
                
                // Si l'utilisateur a sélectionné un tag valide (différent de "Annuler")
                if (selectedTags != null && selectedTags != "Annuler")
                {
                    // Trouver le tag sélectionné
                    var selectedTag = AvailableTags.FirstOrDefault(t => t.Name == selectedTags);
                    
                    if (selectedTag != null)
                    {
                        // Initialiser la collection de tags si elle est nulle
                        if (book.Tags == null)
                        {
                            book.Tags = new List<Tag>();
                        }
                        
                        // Vérifier si le tag n'est pas déjà associé au livre
                        if (!book.Tags.Any(t => t.Id == selectedTag.Id))
                        {
                            // Ajouter le tag au livre en mémoire
                            book.Tags.Add(selectedTag);
                            
                            // Persister la relation livre-tag en base de données
                            bool success = await _bookTagApi.AddTagToBookAsync(book.Id, selectedTag.Id);
                            
                            if (success)
                            {
                                // Actualiser l'affichage
                                OnPropertyChanged(nameof(FilteredBooks));
                                
                                // Feedback à l'utilisateur
                                await App.Current.MainPage.DisplayAlert("Succès", $"Tag '{selectedTag.Name}' ajouté au livre.", "OK");
                            }
                            else
                            {
                                // En cas d'échec, supprimer le tag de la collection en mémoire
                                book.Tags.Remove(selectedTag);
                                await App.Current.MainPage.DisplayAlert("Erreur", "Impossible de sauvegarder l'association du tag avec le livre dans la base de données.", "OK");
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Information", "Ce tag est déjà associé à ce livre.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du tag : {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Erreur", "Impossible d'ajouter le tag.", "OK");
            }
        }
    }
}
