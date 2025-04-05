using App.Services;
using App.ViewModels;

namespace App.Pages
{
    public partial class MainPage : ContentPage
    {
        HttpClient _httpClient = new HttpClient();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new BookViewModel();
        }

    }

}
