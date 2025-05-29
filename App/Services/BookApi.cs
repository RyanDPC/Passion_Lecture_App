using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;
using System.Linq;

namespace App.Services
{
    public class BookApi
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:443/")
        };

        private static byte[] ReadByteArray(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            {
                return Array.Empty<byte>();
            }

            try
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    if (element.TryGetProperty("type", out var typeElem) &&
                        element.TryGetProperty("data", out var dataElem))
                    {
                        var bytes = dataElem.EnumerateArray()
                            .Select(x => (byte)x.GetInt32())
                            .ToArray();
                        return bytes;
                    }
                }
                else if (element.ValueKind == JsonValueKind.Array)
                {
                    var bytes = element.EnumerateArray()
                        .Select(x => (byte)x.GetInt32())
                        .ToArray();
                    return bytes;
                }
            }
            catch
            {
            }

            return Array.Empty<byte>();
        }

        private List<Book> ParseBooks(string json)
        {
            var books = new List<Book>();
            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("book", out JsonElement booksElement))
                {
                    return books;
                }

                foreach (var el in booksElement.EnumerateArray())
                {
                    try
                    {
                        var book = new Book
                        {
                            Id = el.GetProperty("id").GetInt32(),
                            Name = el.GetProperty("name").GetString(),
                            Passage = el.TryGetProperty("passage", out var passage) ? passage.GetString() : null,
                            Summary = el.GetProperty("summary").GetString(),
                            EditionYear = el.GetProperty("editionYear").GetInt32(),
                            Pages = el.TryGetProperty("pages", out var pages) ? pages.GetInt32() : 0,
                            Created = el.TryGetProperty("created", out var created) ? created.GetDateTime() : DateTime.UtcNow,
                            CategoryFk = el.TryGetProperty("category_fk", out var catFk) ? catFk.GetInt32() : 0,
                            EditorFk = el.TryGetProperty("edition_fk", out var editorFk) ? editorFk.GetInt32() : 0,
                            AuthorFk = el.TryGetProperty("author_fk", out var authorFk) ? authorFk.GetInt32() : 0
                        };

                        if (el.TryGetProperty("content", out var contentsElement) &&
                            contentsElement.ValueKind != JsonValueKind.Null &&
                            contentsElement.ValueKind != JsonValueKind.Undefined)
                        {
                            book.Content = ReadByteArray(contentsElement);
                        }

                        if (el.TryGetProperty("coverImage", out var coverImageElement) &&
                            coverImageElement.ValueKind == JsonValueKind.String)
                        {
                            book.CoverImage = coverImageElement.GetString();
                        }
                        else
                        {
                            book.CoverImage = null;
                        }

                        books.Add(book);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return books;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/book");
                var data = await response.Content.ReadAsStringAsync();
                var books = ParseBooks(data);
                return books;
            }
            catch
            {
                return new List<Book>();
            }
        }

        public async Task<List<Book>> GetBooksByNameAsync(string name)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/book?name={name}");
                var data = await response.Content.ReadAsStringAsync();
                return ParseBooks(data);
            }
            catch
            {
                return new List<Book>();
            }
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/book/{id}");
                var data = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(data);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("id", out _))
                {
                    var book = new Book
                    {
                        Id = root.GetProperty("id").GetInt32(),
                        Name = root.GetProperty("name").GetString(),
                        Passage = root.TryGetProperty("passage", out var passage) ? passage.GetString() : null,
                        Summary = root.GetProperty("summary").GetString(),
                        EditionYear = root.GetProperty("editionYear").GetInt32(),
                        Pages = root.TryGetProperty("pages", out var pages) ? pages.GetInt32() : 0,
                        Created = root.TryGetProperty("created", out var created) ? created.GetDateTime() : DateTime.UtcNow,
                        CategoryFk = root.TryGetProperty("category_fk", out var catFk) ? catFk.GetInt32() : 0,
                        EditorFk = root.TryGetProperty("edition_fk", out var editorFk) ? editorFk.GetInt32() : 0,
                        AuthorFk = root.TryGetProperty("author_fk", out var authorFk) ? authorFk.GetInt32() : 0
                    };

                    if (root.TryGetProperty("content", out var contentsElement))
                    {
                        book.Content = ReadByteArray(contentsElement);
                    }

                    return book;
                }
                else
                {
                    var books = ParseBooks(data);
                    return books.FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Book>> GetBooksByDateAddedAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/book?sortByDate=true");
                var data = await response.Content.ReadAsStringAsync();
                return ParseBooks(data);
            }
            catch
            {
                return new List<Book>();
            }
        }

        public async Task<List<Book>> GetBooksByUserIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/book?userId={userId}");
                var data = await response.Content.ReadAsStringAsync();
                return ParseBooks(data);
            }
            catch
            {
                return new List<Book>();
            }
        }

        public async Task CreateBookAsync(Book book)
        {
            try
            {
                var json = JsonSerializer.Serialize(book);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("api/book", content);
            }
            catch
            {
            }
        }
    }
}
