using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;
using Microsoft.Extensions.Logging;

namespace App.Services
{
    public class BookApi
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookApi> _logger;

        public BookApi(IHttpClientFactory httpClientFactory, ILogger<BookApi> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("http://10.0.2.2:443/api/book");
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response);
                return apiResponse?.Book ?? new List<Book>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des livres");
                return new List<Book>();
            }
        }
    }

    public class ApiResponse
    {
        public string Message { get; set; }
        public List<Book> Book { get; set; }
    }
}
