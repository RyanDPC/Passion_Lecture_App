using System;
using System.Collections.Generic;
using System.Net.Http;
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
                var url = $"api/search?query={query}&searchType=book";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(data))
                        return new List<Book>();

                    var books = JsonSerializer.Deserialize<List<Book>>(data, _jsonOptions);
                    return books ?? new List<Book>();
                }
                else
                {
                    return new List<Book>();
                }
            }
            catch
            {
                return new List<Book>();
            }
        }
    }
}
