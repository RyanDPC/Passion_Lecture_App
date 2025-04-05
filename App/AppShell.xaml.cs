
using App.Pages;

namespace App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("SearchPage", typeof(SearchPage));
            Routing.RegisterRoute("CreatePage", typeof(CreatePage));
            Routing.RegisterRoute("ProgilePage", typeof(ProfilePage));
        }
    }
}
