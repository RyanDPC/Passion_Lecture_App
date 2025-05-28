using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using App.Services;
using App.Models;
using App.Pages.Content;
namespace App.ViewModels;

public partial class BookViewModel : ObservableObject
{
    [ObservableProperty] public int id;
    [ObservableProperty] public string name;
    [ObservableProperty] public byte[] coverImage;
    [ObservableProperty] public ObservableCollection<TagViewModel> tags = new();
    [ObservableProperty]public ObservableCollection<Book> books = new();
    public Action<TagViewModel>? NewTagAvaible { get; set; }
    [ObservableProperty] public string ratingBook;
    private readonly BookApi _bookApi;

    public ICommand GoToReadBookCommand => new RelayCommand<int>(GoToReadBook);
    public IAsyncRelayCommand LoadBooksCommand { get; }

    public BookViewModel(){
        _bookApi = new BookApi(); // ou injecté si tu préfères via DI
        LoadBooksCommand = new AsyncRelayCommand(LoadBooksAsync);
    }

    private async void GoToReadBook(int bookId)
    {
        await Shell.Current.GoToAsync($"///readbook?bookId={bookId}");
        Console.WriteLine($"Go to book with ID {bookId}");
    }
    private async Task LoadBooksAsync()
    {
        var livres = await _bookApi.GetAllBooksAsync();
        Books.Clear();
        foreach (var livre in livres)
            Books.Add(livre);
    }
    [RelayCommand]
    private async void AddTagToBookAsync()
    {
        string result = await Application.Current.MainPage.DisplayPromptAsync(
          "Ajouter un tag",
          $"Entrez un nom pour le tag du livre \"{Name}\" :",
          "Valider",
          "Annuler",
          placeholder: "Ex: Fantasy"
        );

        if (!string.IsNullOrWhiteSpace(result))
        {
            var nouveauTag = new TagViewModel(result)
            {
                Id = 0,
                Name = result,
            };

            // Ajouter au livre
            Tags.Add(nouveauTag);

            // Tenter d'ajouter au global via callback
            NewTagAvaible?.Invoke(nouveauTag);

            Console.WriteLine($"Tag '{result}' ajouté au livre : {Name}");
        }
    }

}
