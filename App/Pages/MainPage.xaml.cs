using App.Services;
using App.ViewModels;

namespace App.Pages
{
    public partial class MainPage : ContentPage
    {       
        public MainPage(BookViewModel bookViewModel)
        {
            InitializeComponent();
            BindingContext = bookViewModel;
        }

    }

}
