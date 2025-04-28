using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text.Json;

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

                Books.Clear();
                if (result.Count == 0)
                {
                    Console.WriteLine("Aucun livre trouvé.");
                }

                foreach (var book in result)
                {
                    string imageName = $"/Resources/Images/a{book.Id}.png";
                    book.CoverImage = ImageSource.FromFile(imageName);

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
            
        public async Task AddBookAsync(string name, string passage, string summary, int editionYear, ImageSource coverImage)
        {
            try
            {

                var newBook = new Book
                {
                    Name = name,
                    Passage = passage,
                    Summary = summary,
                    EditionYear = editionYear,
                    CoverImage = coverImage
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
