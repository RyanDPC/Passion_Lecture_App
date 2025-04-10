using System;
using System.Reflection.Metadata;

namespace App.Models
{
    public enum RatingBook
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }
    public enum RatingComments
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }
    public class Book
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Passage { get; set; }
        public int EditionYear { get; set; }
        public int editionYear { get; set; }
        public Blob CoverImage { get; set; }
        public int Pages { get; set; }
        public int CategoryFk { get; set; }
        public int EditorFk { get; set; }
        public int AuthorFk { get; set; }
        public int? UserFk { get; set; }
        public DateTime Created { get; set; }
    }
}