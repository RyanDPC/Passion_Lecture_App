using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls;

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
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("passage")]
        public string Passage { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("editionYear")]
        public int EditionYear { get; set; }

        [JsonPropertyName("coverImage")]
        public byte[] CoverImage { get; set; }
        public int Pages { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("category_fk")]
        public int CategoryFk { get; set; }

        [JsonPropertyName("edition_fk")]
        public int EditorFk { get; set; }

        [JsonPropertyName("author_fk")]
        public int AuthorFk { get; set; }

        [JsonPropertyName("user_fk")]
        public int? UserFk { get; set; }
    }

    public class ApiResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("book")]
        public List<Book> Book { get; set; }
    }

}
