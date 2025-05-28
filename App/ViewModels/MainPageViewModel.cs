using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        public ObservableCollection<Tag> Tags { get; set; }
        public ObservableCollection<Book> Books { get; set; }

        [ObservableProperty]
        private ObservableCollection<Book> filteredBooks;

        public MainPageViewModel()
        {
            // Mock tags
            Tags = new ObservableCollection<Tag>
            {
                new Tag { Id = 1, Name = "Fantasy" },
                new Tag { Id = 2, Name = "Science-Fiction" },
                new Tag { Id = 3, Name = "Romance" },
                new Tag { Id = 4, Name = "Thriller" },
                new Tag { Id = 5, Name = "Historique" }
            };

            foreach (var tag in Tags)
            {
                tag.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Tag.IsSelected))
                        FilterBooks();
                };
            }

            // Mock books
            Books = new ObservableCollection<Book>
            {
                new Book { Id = 1, Name = "Le Seigneur des Anneaux", Summary = "Un roman de fantasy.", Tags = new[] { Tags[0] } },
                new Book { Id = 2, Name = "Dune", Summary = "Science-fiction épique.", Tags = new[] { Tags[1] } },
                new Book { Id = 3, Name = "Orgueil et Préjugés", Summary = "Roman classique de romance.", Tags = new[] { Tags[2] } },
                new Book { Id = 4, Name = "Da Vinci Code", Summary = "Thriller mystérieux.", Tags = new[] { Tags[3] } },
                new Book { Id = 5, Name = "Les Misérables", Summary = "Roman historique.", Tags = new[] { Tags[4] } }
            };

            FilteredBooks = new ObservableCollection<Book>(Books);
        }

        private void FilterBooks()
        {
            var selectedTags = Tags.Where(t => t.IsSelected).Select(t => t.Name).ToList();
            if (selectedTags.Count == 0)
            {
                FilteredBooks = new ObservableCollection<Book>(Books);
            }
            else
            {
                var filtered = Books.Where(b => b.Tags.Any(t => selectedTags.Contains(t.Name))).ToList();
                FilteredBooks = new ObservableCollection<Book>(filtered);
            }
            OnPropertyChanged(nameof(FilteredBooks));
        }
    }
}
