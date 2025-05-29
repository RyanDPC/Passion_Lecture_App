using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using App.Models;
using App.Services;

namespace App.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<BookViewModel> filteredBooks = new();

    [ObservableProperty]
    ObservableCollection<TagViewModel> availableTags = new();

    private List<BookViewModel> allBooks = new();

    [ObservableProperty]
    private string newTagName;

    public MainPageViewModel()
    {
        LoadTags();
        LoadBooksAsync();
        FilterBooks();
    }

    private void LoadTags()
    {
        AvailableTags = new ObservableCollection<TagViewModel>
        {
            new TagViewModel("Roman"),
            new TagViewModel("Fantasy"),
            new TagViewModel("Science-Fiction"),
            new TagViewModel("Horreur")
        };
    }

    private async Task LoadBooksAsync()
    {
        var bookApi = new BookApi();
        var booksFromApi = await bookApi.GetAllBooksAsync();

        allBooks.Clear();
        foreach (var book in booksFromApi)
        {
            var bookVM = new BookViewModel
            {
                Id = book.Id,
                Name = book.Name,
                CoverImage = book.CoverImage,
                Content = book.Content,
            };

            bookVM.NewTagAvaible = HandleNewTagGlobally;

            allBooks.Add(bookVM);
        }

        FilterBooks();
    }

    [RelayCommand]
    private void TagSelectionChanged(TagViewModel selectedTag)
    {
        foreach (var tag in AvailableTags)
            tag.IsSelected = tag == selectedTag;

        FilterBooks();
    }

    public void OnTagCheckboxChanged()
    {
        FilterBooks();
    }

    private void FilterBooks()
    {
        var selectedTags = AvailableTags.Where(t => t.IsSelected).Select(t => t.Name).ToList();

        if (!selectedTags.Any())
        {
            FilteredBooks = new ObservableCollection<BookViewModel>(allBooks);
        }
        else
        {
            var filtered = allBooks.Where(book =>
                book.Tags.Any(tag => selectedTags.Contains(tag.Name))
            ).ToList();

            FilteredBooks = new ObservableCollection<BookViewModel>(filtered);
        }
    }

    private void HandleNewTagGlobally(TagViewModel newTag)
    {
        if (!AvailableTags.Any(tag => tag.Name.Equals(newTag.Name, StringComparison.OrdinalIgnoreCase)))
        {
            AvailableTags.Add(new TagViewModel(newTag.Name));
        }
    }
}
