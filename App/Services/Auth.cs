using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;
using App.Interface;
using App.ViewModels;

namespace App.Services
{
    public class Auth : IAuthInterface
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://10.0.2.2:443/api"; // Base URL

        // Constructeur
        public Auth(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Méthode générique pour envoyer des requêtes HTTP
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

        // Implémentation de la méthode LoginAsync
        public Task<Models.User> LoginAsync(string username, string password)
        {
            var data = new { username, password };
            return SendRequestAsync<Models.User>(HttpMethod.Post, "auth/login", data);
        }

        // Implémentation de la méthode RegisterAsync
        public Task<Models.User> RegisterAsync(string username, string password, string email)
        {
            var data = new { username, password, email };
            return SendRequestAsync<Models.User>(HttpMethod.Post, "auth/register", data);
        }

        // Implémentation de la méthode LogoutAsync
        public Task<bool> LogoutAsync()
        {
            return SendRequestAsync<bool>(HttpMethod.Post, "auth/logout", null);
        }

        // Implémentation de la méthode GetUserAsync
        public Task<Models.User> GetUserAsync(int userId)
        {
            return SendRequestAsync<Models.User>(HttpMethod.Get, $"users/{userId}");
        }

        // Implémentation de la méthode UpdateUserAsync
        public Task<bool> UpdateUserAsync(Models.User user)
        {
            return SendRequestAsync<bool>(HttpMethod.Put, $"users/{user.Id}", user);
        }

        // Implémentation de la méthode DeleteUserAsync
        public Task<bool> DeleteUserAsync(int userId)
        {
            return SendRequestAsync<bool>(HttpMethod.Delete, $"users/{userId}");
        }

        // Implémentation des méthodes de l'interface IAuthInterface pour le ViewModel User

        Task<ViewModels.User> IAuthInterface.LoginAsync(string username, string password)
        {
            var data = new { username, password };
            return SendRequestAsync<ViewModels.User>(HttpMethod.Post, "auth/login", data);
        }

        Task<ViewModels.User> IAuthInterface.RegisterAsync(string username, string password, string email)
        {
            var data = new { username, password, email };
            return SendRequestAsync<ViewModels.User>(HttpMethod.Post, "auth/register", data);
        }

        Task<bool> IAuthInterface.LogoutAsync()
        {
            return SendRequestAsync<bool>(HttpMethod.Post, "auth/logout", null);
        }

        Task<ViewModels.User> IAuthInterface.GetUserAsync(int userId)
        {
            return SendRequestAsync<ViewModels.User>(HttpMethod.Get, $"users/{userId}");
        }

        Task<bool> IAuthInterface.UpdateUserAsync(Models.User user)
        {
            return SendRequestAsync<bool>(HttpMethod.Put, $"users/{user.Id}", user);
        }

        Task<bool> IAuthInterface.DeleteUserAsync(int userId)
        {
            return SendRequestAsync<bool>(HttpMethod.Delete, $"users/{userId}");
        }
    }
}
