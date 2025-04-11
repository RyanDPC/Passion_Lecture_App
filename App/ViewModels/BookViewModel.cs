using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace App.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Book> books = new();

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

                Console.WriteLine($"Livres récupérés : {result.Count}");

                Books.Clear();
                if (result.Count == 0)
                {
                    Console.WriteLine("Aucun livre trouvé.");
                }

                foreach (var book in result)
                {
                    if (book.CoverImage != null && book.CoverImage.Length > 0)
                    {
                        var imageSource = ImageSource.FromStream(() => new MemoryStream(book.CoverImage));
                        book.ImageSource = imageSource;
                    }
                    Books.Add(book);
                }

                Console.WriteLine($"Total des livres chargés : {Books.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des livres: {ex.Message}");
            }
        }

        // Convertir une image en base64
        public string ConvertImageToBase64(ImageSource imageSource)
        {
            try
            {
                if (imageSource is FileImageSource fileImageSource)
                {
                    var imagePath = fileImageSource.File;
                    var imageBytes = File.ReadAllBytes(imagePath);
                    return Convert.ToBase64String(imageBytes);
                }

                if (imageSource is StreamImageSource streamImageSource)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var stream = streamImageSource.Stream(CancellationToken.None).Result;
                        stream.CopyTo(memoryStream);
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de conversion de l'image : {ex.Message}");
            }

            return null;
        }

        // Ajouter un livre avec une image en base64
        public async Task AddBookAsync(string name, string passage, string summary, int editionYear, byte[] coverImage)
        {
            try
            {
                // Convertir l'image en Base64
                var coverImageBase64 = Convert.ToBase64String(coverImage);

                var newBook = new Book
                {
                    Name = name,
                    Passage = passage,
                    Summary = summary,
                    EditionYear = editionYear,
                    CoverImage = coverImage,
                    CoverImageBase64 = coverImageBase64
                };

                await _bookApi.CreateBookAsync(newBook);

                Books.Add(newBook);

                Console.WriteLine($"Livre '{name}' ajouté avec succès.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du livre : {ex.Message}");
            }
        }
    }
}
