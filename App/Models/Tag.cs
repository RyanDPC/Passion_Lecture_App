using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace App.Models
{
    public class Tag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("value")]
        public string Name { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        
        public ICollection<Book> Books { get; set; }
        
        public bool IsSelected { get; set; }
    }
}
