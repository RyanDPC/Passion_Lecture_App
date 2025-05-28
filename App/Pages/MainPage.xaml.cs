using App.ViewModels;
using Microsoft.Maui.Controls;

namespace App.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel(); // Assure-toi que c’est bien fait ici
        }

        private void TagSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BindingContext is MainPageViewModel vm && e.CurrentSelection.FirstOrDefault() is TagViewModel tag)
            {
                vm.TagSelectionChangedCommand.Execute(tag);
            }
        }

        private void TagCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (BindingContext is MainPageViewModel vm)
                vm.OnTagCheckboxChanged();
        }
    }
}
