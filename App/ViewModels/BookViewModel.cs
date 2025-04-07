using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Services;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;

namespace App.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        private readonly BookApi _api;

        // Propriété observable pour les livres
        [ObservableProperty]
        private ObservableCollection<Book> books = new();

        // Commande pour charger les livres
        [RelayCommand]
        private async Task LoadBooksAsync()
        {
            try
            {
                var allBooks = await _api.GetAllBooksAsync();
                Books = new ObservableCollection<Book>((IEnumerable<Book>)allBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur API : {ex.Message}");
            }
        }

        // Constructeur pour l'injection de dépendances
        public BookViewModel(BookApi api)
        {
            _api = api;
            LoadBooksCommand.Execute(null); // Charge les livres au démarrage
        }

        // Commande asynchrone pour charger les livres
        public IAsyncRelayCommand LoadBooksCommand { get; }
    }
}
