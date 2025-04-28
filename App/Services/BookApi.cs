using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;
using System.Collections.Generic;

public class BookApi
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://10.0.2.2:8080")
    };

    // Récupérer les livres par nom
    public async Task<List<Book>> GetBooksByNameAsync(string name)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/book?name={name}");
            var data = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(data))
            {
                return new List<Book>();
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(data);

            return apiResponse?.Book ?? new List<Book>();
        }
        catch
        {
            return new List<Book>();
        }
    }

    // Récupérer les livres par date d'ajout
    public async Task<List<Book>> GetBooksByDateAddedAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/book?sortByDate=true");
            var data = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(data))
            {
                return new List<Book>();
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(data);

            return apiResponse?.Book ?? new List<Book>();
        }
        catch
        {
            return new List<Book>();
        }
    }

    // Récupérer les livres créés par un utilisateur spécifique (avec UserFk)
    public async Task<List<Book>> GetBooksByUserIdAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/book?userId={userId}");
            var data = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(data))
            {
                return new List<Book>();
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(data);

            return apiResponse?.Book ?? new List<Book>();
        }
        catch
        {
            return new List<Book>();
        }
    }

    // Récupérer tous les livres
    public async Task<List<Book>> GetAllBooksAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/book");
            var data = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(data))
            {
                return new List<Book>();
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(data);

            return apiResponse?.Book ?? new List<Book>();
        }
        catch
        {
            return new List<Book>();
        }
    }

    // Méthode pour créer un livre
    public async Task CreateBookAsync(Book book)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(book), System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("api/book", content);
        }
        catch
        {
        }
    }
}
