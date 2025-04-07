using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly string _baseUrl = "https://10.0.2.2:443/api";

        // Le constructeur de BookApi
        public BookApi(IHttpClientFactory httpClientFactory)
        {
            string token = "gsgsdsgds";
            _httpClient = httpClientFactory.CreateClient("api");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // GET : Récupérer tous les livres
        public async Task<Book> GetAllBooksAsync()
        {
            var response = await _httpClient.GetAsync("api/book");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erreur lors de la récupération des livres");

            var json = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<Book>(json);

            return book!;
        }
        // GET : Récupérer un livre par son ID
        public async Task<Book> GetBookByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/book/{id}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erreur lors de la recherche d'un livre");

            var json = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<Book>(json);

            return book!;
        }
        // POST : Créer un livre
        public async Task<Book> CreateBookAsync(Book createbook, bool newBook = false)
        {
            try
            {
                var json = JsonSerializer.Serialize(createbook, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                response = await _httpClient.PostAsync("api/book/add", content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Erreur lors de la création d'un livre");

                var responseBody = await response.Content.ReadAsStringAsync();
                var createdBook = JsonSerializer.Deserialize<Book>(responseBody, _serializerOptions);

                return createdBook!;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw; // ou return null si tu préfères, mais attention à gérer ça dans l’appelant
            }
        }

        // PUT : Mettre à jour un livre
        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            var json = JsonSerializer.Serialize<Book>(book, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/book/{id}", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erreur lors de la recherche d'un livre");
            return book!;
        }
        public async Task<Book> DeleteBookAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/book/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Erreur lors de la recherche d'un livre");

            var json = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<Book>(json);
            return book!;
        }
    }
}
