using App.ViewModels;
using Microsoft.Maui.Controls;

namespace App.Pages.Content;

public partial class ReadBookPage : ContentPage, IQueryAttributable
{
        private ReadBookViewModel viewModel;

    public ReadBookPage()
    {
        InitializeComponent();
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("bookId", out var bookIdObj) && int.TryParse(bookIdObj.ToString(), out int bookId))
        {
            viewModel = new ReadBookViewModel(bookId);
            BindingContext = viewModel;

            // Charger les données de façon asynchrone, sans bloquer
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await viewModel.LoadBookAsync(bookId);
            });
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Tu peux faire d'autres initialisations ici si besoin
    }
}
