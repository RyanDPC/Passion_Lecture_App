using System;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

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
            public int Id { get; set; }
            public string Name { get; set; }
            public string Passage { get; set; }
            public string Summary { get; set; }
            public int EditionYear { get; set; }
            public string CoverImage { get; set; }
            public int Pages { get; set; }
            public DateTime Created { get; set; }
            public int CategoryFk { get; set; }
            public int EditorFk { get; set; }
            public int AuthorFk { get; set; }
            public int? UserFk { get; set; }
    }
    public class ApiResponse
        {         
        public String Message { get; set; }

        [JsonPropertyName("book")]
        public List<Book> Book { get; set; }
        }
}