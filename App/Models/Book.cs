using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.Controls;
using App.Helper;
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
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(255, ErrorMessage = "Le nom ne peut pas dépasser 255 caractères")]
        [RegularExpression(@"^[\p{L}\p{P}\s\d]*$", ErrorMessage = "Seules les lettres, les accents, les espaces, les virgules et les points sont autorisés")]
        public string Name { get; set; }

        [JsonPropertyName("passage")]
        [StringLength(250, ErrorMessage = "Le passage ne peut pas dépasser 250 caractères")]
        [RegularExpression(@"^[\p{L}\p{P}\s\d]*$", ErrorMessage = "Seules les lettres, les accents, les espaces, les virgules et les points sont autorisés")]
        public string Passage { get; set; }

        [JsonPropertyName("summary")]
        [Required(ErrorMessage = "Le résumé est obligatoire")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Le résumé doit faire entre 1 et 2000 caractères")]
        public string Summary { get; set; }

        [JsonPropertyName("editionYear")]
        [Range(0, 9999)]
        public int EditionYear { get; set; }

        [JsonPropertyName("coverImage")]
        public byte[] CoverImage { get; set; }

        [JsonPropertyName("content")]
        [JsonConverter(typeof(BufferToByteArrayConverter))]
        public byte[] Content { get; set; }

        [JsonPropertyName("pages")]
        [Range(0, int.MaxValue)]
        public int Pages { get; set; }

        [JsonPropertyName("created")]
        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("category_fk")]
        public int? CategoryFk { get; set; }

        [JsonPropertyName("edition_fk")]
        public int? EditorFk { get; set; }

        [JsonPropertyName("author_fk")]
        public int? AuthorFk { get; set; }

        public int LastReadPage { get; set; } = 1;
        public string? LastReadChapter { get; set; }
        public double? LastReadPosition { get; set; } = 0;
        public DateTime? LastReadDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("tags")]
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }

        public string GetContentDebugInfo()
        {
            return $"Content is null: {Content == null}, " +
                   $"Length: {Content?.Length ?? 0}, " +
                   $"First few bytes: {(Content != null && Content.Length > 0 ? 
                       BitConverter.ToString(Content.Take(5).ToArray()) : "none")}";
        }
    }

    public class ApiResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("book")]
        public List<Book> Book { get; set; }
    }

}
