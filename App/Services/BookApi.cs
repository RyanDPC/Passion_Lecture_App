using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;

public class BookApi
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://10.0.2.2:443")
    };
        public async Task<List<Book>> GetBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/book");
                var data = await response.Content.ReadAsStringAsync();

                // Affiche la réponse brute de l'API pour débogage
                Console.WriteLine("Réponse brute de l'API : " + data);

                if (string.IsNullOrEmpty(data))
                {
                    Console.WriteLine("Erreur de sérialisation ou liste vide");
                    return new List<Book>();
                }

                // Désérialisation de la réponse JSON en ApiResponse
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(data);

                // Vérifier si la liste des livres existe et est valide
                if (apiResponse == null || apiResponse.Book == null || apiResponse.Book.Count == 0)
                {
                    Console.WriteLine("Aucun livre trouvé.");
                    return new List<Book>();
                }

                return apiResponse.Book;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la requête HTTP : {ex.Message}");
                return new List<Book>();
            }
        }
    }
