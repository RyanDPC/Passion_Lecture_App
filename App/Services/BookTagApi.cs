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
                // D'après le backend, pour les tags on doit envoyer un array de valeurs (names) et non d'IDs
                // Récupérer d'abord le nom du tag à partir de son ID
                var tagName = await GetTagNameById(tagId);
                if (string.IsNullOrEmpty(tagName))
                {
                    Console.WriteLine($"Tag avec ID {tagId} non trouvé");
                    return false;
                }
                
                // Structure correcte selon la fonction Update du backend
                var bookUpdate = new
                {
                    tags = new[] { tagName } // Le backend attend les valeurs du tag, pas les IDs
                };

                // Sérialiser en JSON avec options spécifiques
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(bookUpdate, options);
                Console.WriteLine($"Envoi de la requête: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envoyer la requête PUT à l'API pour mettre à jour le livre
                var response = await _httpClient.PutAsync($"api/book/{bookId}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur API: {errorMessage}");
                    return false;
                }
                
                Console.WriteLine($"Tag {tagName} (ID: {tagId}) ajouté au livre {bookId} avec succès");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du tag au livre : {ex.Message}");
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
                        // Le backend renvoie probablement un objet tag avec une propriété "value"
                        if (doc.RootElement.TryGetProperty("value", out var valueElement))
                        {
                            return valueElement.GetString();
                        }
                    }
                }
                
                // Plan B: Utiliser les données locales
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du nom du tag: {ex.Message}");
                return null;
            }
        }

        // Supprimer l'association entre un tag et un livre
        public async Task<bool> RemoveTagFromBookAsync(int bookId, int tagId)
        {
            try
            {
                // Comme il n'y a pas d'endpoint spécifique pour supprimer un tag,
                // on devrait plutôt mettre à jour le livre avec un tableau de tags excluant celui à supprimer
                // Cette implémentation nécessiterait de récupérer d'abord la liste actuelle des tags
                
                // Pour l'instant, cette fonctionnalité n'est pas implémentée côté backend
                Console.WriteLine($"Suppression de l'association entre le tag {tagId} et le livre {bookId} non supportée par l'API");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression du tag du livre : {ex.Message}");
                return false;
            }
        }
    }
} 