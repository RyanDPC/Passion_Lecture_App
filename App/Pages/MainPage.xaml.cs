using App.ViewModels;
using App.Models;
using System.Linq;
using Microsoft.Maui.Controls;

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

        private void TagCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // Appelle une méthode du ViewModel pour mettre à jour le filtre
            (BindingContext as BookViewModel)?.UpdateTagFilterAsync();
        }
    }
}
