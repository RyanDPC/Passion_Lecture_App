using System.Collections.ObjectModel;
using System.Threading.Tasks;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;

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

        private async Task LoadBooksAsync()
        {
            try
            {
                var result = await _bookApi.GetBooksAsync();

                // Vérifie si la requête renvoie des livres
                Console.WriteLine($"Livres récupérés : {result.Count}");

                Books.Clear();
                if (result.Count == 0)
                {
                    Console.WriteLine("Aucun livre trouvé.");
                }

                foreach (var book in result)
                {
                    Console.WriteLine($"Livre ajouté : {book.Name}");
                    Books.Add(book);
                }

                Console.WriteLine($"Total des livres chargés : {Books.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des livres: {ex.Message}");
            }
        }

    }
}