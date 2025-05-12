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

        [ObservableProperty] private string name;
        [ObservableProperty] private string summary;
        [ObservableProperty] private string passage;
        [ObservableProperty] private string editionYear;
        [ObservableProperty] private ImageSource coverImage;
        private byte[] imageBytes;

        private readonly BookApi _bookApi = new();
        public BookViewModel()
        {
            LoadBooksAsync();
        }
        // Charger les livres depuis l'API
        private async Task LoadBooksAsync()
        {
            try
            {
                var result = await _bookApi.GetAllBooksAsync();
                Console.Write(result);
                Books.Clear();
                if (result.Count == 0)
                {
                    Console.WriteLine("Aucun livre trouvé.");
                }

                foreach (var book in result)
                {
                    Books.Add(book);

                    Debug.WriteLine($"Livre ajouté : {book.Name}");
                    string json = JsonSerializer.Serialize(book);
                    Debug.WriteLine($"Contenu JSON du livre : {json}");
                }

                Console.WriteLine($"Total des livres chargés : {Books.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des livres: {ex.Message}");
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
                // Naviguer vers la page de lecture du livre avec l'ID du livre dans l'URL
                await Shell.Current.GoToAsync($"ReadBookPage?bookId={bookId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la navigation vers le livre : {ex.Message}");
            }
        }

    }
}
