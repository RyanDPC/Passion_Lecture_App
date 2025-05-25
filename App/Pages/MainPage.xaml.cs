using App.ViewModels;
using App.Models;
using System.Linq;

namespace App.Pages
{
    public partial class MainPage : ContentPage
    {       
        public MainPage()
        {
            InitializeComponent();
        }

        private void TagSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BindingContext is BookViewModel vm && e.CurrentSelection.FirstOrDefault() is Tag tag)
            {
                vm.ToggleTagCommand.Execute(tag);
            }
        }
    }
}
