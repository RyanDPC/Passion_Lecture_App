using App.Services;
using App.ViewModels;
using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using VersOne.Epub;

namespace App.Pages.Content;

public partial class ReadBookPage : ContentPage, IQueryAttributable
{
    private ReadBookViewModel viewModel;

    public ReadBookPage()
    {
        InitializeComponent();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("bookId", out var bookIdObj) && int.TryParse(bookIdObj.ToString(), out int bookId))
        {
            viewModel = new ReadBookViewModel(bookId);
            BindingContext = viewModel;

            await LoadBookAndEpubAsync();
        }
    }

    private async Task LoadBookAndEpubAsync()
    {
        await viewModel.LoadBookAsync();

        if (viewModel.CurrentBook?.Content != null && viewModel.CurrentBook.Content.Length > 0)
        {
            try
            {
                using var ms = new MemoryStream(viewModel.CurrentBook.Content);
                var epubBook = await EpubReader.ReadBookAsync(ms);

                // Ajout de logs pour le débogage
                foreach (var chapter in epubBook.ReadingOrder)
                {
                }

                await viewModel.LoadChaptersFromEpubAsync(epubBook);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur EPUB", $"Impossible de lire le livre : {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Aucun contenu", "Ce livre ne contient pas de fichier EPUB.", "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await viewModel?.SaveReadingProgressAsync();
    }
}
