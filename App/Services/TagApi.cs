using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;
using System.Collections.Generic;
using App.Helper;
namespace App.Services
{
    public class TagApi
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:443/")
        };

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/tags");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(data))
                {
                    return new List<Tag>();
                }

                var options = new JsonSerializerOptions();
                options.Converters.Add(new NodeBufferToByteArrayConverter());
                var tags = JsonSerializer.Deserialize<List<Tag>>(data, options);
                return tags ?? new List<Tag>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des tags : {ex.Message}");
                return new List<Tag>();
            }
        }
    }
} 