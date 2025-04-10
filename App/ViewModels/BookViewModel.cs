using System.Collections.ObjectModel;
using System.Threading.Tasks;
using App.Models;
using App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace App.ViewModels
{
    public class BookViewModel : BindableObject
    {
        private ObservableCollection<Book> _books;
        private readonly BookApi _bookApi;

        public ObservableCollection<Book> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged();
            }
        }

        public BookViewModel()
        {
            // Utilisation du service BookApi via l'injection de dépendances (via IServiceProvider)
            _bookApi = DependencyService.Get<BookApi>();
            _books = new ObservableCollection<Book>();
            LoadBooksAsync();
        }

        private async Task LoadBooksAsync()
        {
            var books = await _bookApi.GetBooksAsync();
            Books.Clear();
            foreach (var book in books)
            {
                Books.Add(book);
            }
        }
    }
}
