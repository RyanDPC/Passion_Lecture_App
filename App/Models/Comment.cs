using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace App.Models
{
    public class Comment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("note")]
        public int Note { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("user_fk")]
        public int UserFk { get; set; }
        
        [JsonPropertyName("book_fk")]
        public int BookFk { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        public Book Book { get; set; }
        public User User { get; set; }
    }
} 