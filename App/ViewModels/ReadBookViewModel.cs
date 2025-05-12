using App.Database;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace App.ViewModels
{
    public partial class ReadBookViewModel : ObservableObject
    {
        // On retire le private readonly int _bookId
        private int _bookId;

        [ObservableProperty]
        private Book book;

        [ObservableProperty]
        private int currentPage;

        // Constructeur avec un paramètre bookId
        public ReadBookViewModel(int bookId)
        {
            _bookId = bookId;
            LoadBookAsync(bookId).Wait();
        }

        // Charger le livre et la page en cours
        public async Task LoadBookAsync(int bookId)
        {
            await using var context = new DataContext();
            Book = await context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            CurrentPage = Book?.LastReadPage ?? 1;  // Si la page n'est pas définie, commence à la page 1
        }

        // Passer à la page suivante
        [RelayCommand]
        public async Task NextPage()
        {
            if (CurrentPage < Book.Pages)
            {
                CurrentPage++;
                await SaveProgressAsync();
            }
        }

        // Revenir à la page précédente
        [RelayCommand]
        public async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await SaveProgressAsync();
            }
        }

        // Sauvegarder la progression dans la base de données
        private async Task SaveProgressAsync()
        {
            await using var context = new DataContext();
            var bookToUpdate = await context.Books.FirstOrDefaultAsync(b => b.Id == _bookId);
            if (bookToUpdate != null)
            {
                bookToUpdate.LastReadPage = CurrentPage;
                await context.SaveChangesAsync();
            }
        }
    }

}
