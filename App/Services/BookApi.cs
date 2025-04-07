using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;

namespace App.Services
{
    public class BookApi
    {
        private readonly HttpClient _httpClient;
        JsonSerializerOptions _serializerOptions;
        private readonly string _baseUrl = "https://10.0.2.2:443/api"; // Base URL

        // Le constructeur de BookApi
        public BookApi(string token, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Envoi de la requête HTTP
        private async Task<T> SendRequestAsync<T>(HttpMethod method, string endpoint, object data = null)
        {
            var request = new HttpRequestMessage(method, new Uri(_baseUrl + endpoint))
            {
                Content = data != null ? new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json") : null
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();  // Assure que la réponse a un code de succès

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // GET : Récupérer tous les livres
        public async Task<List<Book>> GetAllBooksAsync()
        {
            var response = await SendRequestAsync<List<Book>>(HttpMethod.Get, "/book/");
            if (response == null || !response.Any())
                throw new Exception("Aucun livre trouvé");

            return response;
        }

        // GET : Récupérer un livre par son ID
        public Task<Book?> GetBookByIdAsync(int id) =>
            SendRequestAsync<Book?>(HttpMethod.Get, $"/book/{id}");

        // POST : Créer un livre
        public Task<bool> CreateBookAsync(Book book) =>
            SendRequestAsync<bool>(HttpMethod.Post, "/book/add", book);

        // PUT : Mettre à jour un livre
        public Task<bool> UpdateBookAsync(int id, Book book) =>
            SendRequestAsync<bool>(HttpMethod.Put, $"/book/{id}", book);

        // DELETE : Supprimer un livre
        public Task<bool> DeleteBookAsync(int id) =>
            SendRequestAsync<bool>(HttpMethod.Delete, $"/book/{id}");
    }
}
