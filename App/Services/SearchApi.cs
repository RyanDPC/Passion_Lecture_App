using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;
using App.Helper;

namespace App.Services
{
    class SearchApi
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:443/")
        };
        private readonly JsonSerializerOptions _jsonOptions;

        public SearchApi()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new NodeBufferToByteArrayConverter());
        }

        public async Task<List<Book>> SearchBook(string query)
        {
            try
            {
                Console.WriteLine($"Envoi de la requête pour rechercher des livres avec : {query}");

                var url = $"api/search?query={query}&searchType=book";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Réponse de l'API : {data}");

                    if (string.IsNullOrWhiteSpace(data))
                        return new List<Book>();

                    var books = JsonSerializer.Deserialize<List<Book>>(data, _jsonOptions);
                    return books ?? new List<Book>();
                }
                else
                {
                    Console.WriteLine($"Erreur dans la requête : {response.StatusCode}");
                    return new List<Book>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la recherche de livres : {ex.Message}");
                return new List<Book>();
            }
        }   
    }
}
