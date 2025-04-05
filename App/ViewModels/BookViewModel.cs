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
    public partial class BookViewModel : ObservableObject  // Rendre la classe publique
    {
        private readonly BookApi _api;

        [ObservableProperty]
        public ObservableCollection<Book> books = new();

        // Constructeur sans paramètre public
        public BookViewModel()
        {
            var httpClient = new HttpClient();  // Crée une instance de HttpClient ici
            string fakeToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MSwidXNlcm5hbWUiOiJFaXRoYW4iLCJpYXQiOjE3NDM4NDUzNjYsImV4cCI6MTc0Mzg0ODk2Nn0.nb5CQJxELSj9FQ_Iv7h_VFCBZwMnas1o1KS7Ifr6BWrAOqADd9XgUrSzvjuCsBkicbXePSkgefokq1peXGa-eKlz5E4Nt8Refcu4rAbydJEW-hTwZeSA2068TnDPD5NrZ4kn21U46RvF87_gSFxAGqiH15I2SJPAi0KBTHe2MJkV60Mr0nvEG4zVegC7MRYHfKcG-EtyrRW2I56k8hmivN0wWi7v68FT76HVcoSwE4PmZxUIcFqWgofvMGIAnkx8XGlV1w20UFx3x7asHNzIyT4O1VetRQ0WdlphSDTKZPbBCJxDhOhoObJNB3hG0JJGRMWr6T1HSQ9M-DlkjSTt_Q";
            _api = new BookApi(fakeToken, httpClient);  // Passer HttpClient à BookApi
            LoadBooksCommand = new AsyncRelayCommand(LoadBooksAsync);
            LoadBooksCommand.Execute(null); // Auto-chargement au démarrage
        }

        public IAsyncRelayCommand LoadBooksCommand { get; }

        private async Task LoadBooksAsync()
        {
            try
            {
                var allBooks = await _api.GetAllBooksAsync();
                Books = new ObservableCollection<Book>(allBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur API : {ex.Message}");
            }
        }
    }
}
