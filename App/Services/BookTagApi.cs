using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using App.Database;

namespace App.Services
{
    public class BookTagApi
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:443/")
        };

        // Associer un tag à un livre dans la base de données
        public async Task<bool> AddTagToBookAsync(int bookId, int tagId)
        {
            try
            {
                var tagName = await GetTagNameById(tagId);
                if (string.IsNullOrEmpty(tagName))
                {
                    return false;
                }

                var bookUpdate = new
                {
                    tags = new[] { tagName }
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(bookUpdate, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/book/{bookId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Récupérer le nom d'un tag à partir de son ID
        private async Task<string> GetTagNameById(int tagId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/tags/{tagId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        if (doc.RootElement.TryGetProperty("value", out var valueElement))
                        {
                            return valueElement.GetString();
                        }
                    }
                }

                using (var context = new DataContext())
                {
                    var tag = await context.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        return tag.Name;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // Supprimer l'association entre un tag et un livre
        public async Task<bool> RemoveTagFromBookAsync(int bookId, int tagId)
        {
            try
            {
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
