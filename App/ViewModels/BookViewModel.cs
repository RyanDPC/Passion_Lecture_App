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
    [ObservableProperty] public string coverImage;
    [ObservableProperty] public byte[] content;
    [ObservableProperty] public ObservableCollection<TagViewModel> tags = new();
    [ObservableProperty] public ObservableCollection<Book> books = new();
    [ObservableProperty] private string searchText;

    public Action<TagViewModel>? NewTagAvaible { get; set; }
    [ObservableProperty] public string ratingBook;

    private readonly BookApi _bookApi;
    private readonly SearchApi _searchApi;

    public ICommand GoToReadBookCommand => new RelayCommand<int>(GoToReadBook);
    public IAsyncRelayCommand LoadBooksCommand { get; }
    public IAsyncRelayCommand SearchBookCommand { get; }

    public BookViewModel()
    {
        _bookApi = new BookApi();
        _searchApi = new SearchApi();

        LoadBooksCommand = new AsyncRelayCommand(LoadBooksAsync);
        SearchBookCommand = new AsyncRelayCommand(SearchBooksAsync);
    }

    private async void GoToReadBook(int bookId)
    {
        await Shell.Current.GoToAsync($"///readbook?bookId={bookId}");
    }

    private async Task LoadBooksAsync()
    {
        var livres = await _bookApi.GetAllBooksAsync();
        Books.Clear();
        foreach (var livre in livres)
            Books.Add(livre);
    }

    private async Task SearchBooksAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            // Optionnel : recharger tous les livres si la recherche est vide
            await LoadBooksAsync();
            return;
        }

        var results = await _searchApi.SearchBook(SearchText);
        Books.Clear();
        foreach (var book in results)
            Books.Add(book);
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

            Tags.Add(nouveauTag);
            NewTagAvaible?.Invoke(nouveauTag);
        }
    }
}
