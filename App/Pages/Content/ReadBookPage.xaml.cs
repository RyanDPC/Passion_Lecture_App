using App.Services;
using App.ViewModels;
namespace App.Pages.Content;

public partial class ReadBookPage : ContentPage
{
    private readonly int _bookId;
    public ReadBookPage()
    {
        InitializeComponent();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

    }

    public ReadBookPage(int bookId)
    {
        InitializeComponent();
        _bookId = bookId;
        BindingContext = new ReadBookViewModel(bookId);
    }
}