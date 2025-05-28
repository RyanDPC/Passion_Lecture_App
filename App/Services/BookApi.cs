using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using App.Models;

namespace App.Services
{
    public class BookApi
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:443/")
        };

        // Méthode pour lire un byte[] depuis une chaîne base64 ou un tableau JSON
        private static byte[] ReadByteArray(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined)
            {
                System.Diagnostics.Debug.WriteLine("[DOTNET] Content is null or undefined, skipping conversion.");
                return Array.Empty<byte>();
            }

            System.Diagnostics.Debug.WriteLine($"[DOTNET] Type de l'élément Content: {element.ValueKind}");
            
            try
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    // Pour le format {type: "Buffer", data: [...]}
                    if (element.TryGetProperty("type", out var typeElem) && 
                        element.TryGetProperty("data", out var dataElem))
                    {
                        System.Diagnostics.Debug.WriteLine("[DOTNET] Format Buffer trouvé");
                        var bytes = dataElem.EnumerateArray()
                            .Select(x => (byte)x.GetInt32())
                            .ToArray();
                        System.Diagnostics.Debug.WriteLine($"[DOTNET] Bytes lus: {bytes.Length}");
                        return bytes;
                    }
                }
                else if (element.ValueKind == JsonValueKind.Array)
                {
                    System.Diagnostics.Debug.WriteLine("[DOTNET] Format tableau trouvé");
                    var bytes = element.EnumerateArray()
                        .Select(x => (byte)x.GetInt32())
                        .ToArray();
                    System.Diagnostics.Debug.WriteLine($"[DOTNET] Bytes lus: {bytes.Length}");
                    return bytes;
                }
                
                System.Diagnostics.Debug.WriteLine($"[DOTNET] Format non reconnu: {element}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DOTNET] Erreur dans ReadByteArray: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[DOTNET] Element problématique: {element}");
            }

            return Array.Empty<byte>();
        }

        // Méthode générique pour désérialiser une liste de livres
        private List<Book> ParseBooks(string json)
        {
            var books = new List<Book>();
            try 
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("book", out JsonElement booksElement))
                {
                    System.Diagnostics.Debug.WriteLine("[DOTNET] Propriété 'book' non trouvée");
                    return books;
                }

                foreach (var el in booksElement.EnumerateArray())
                {
                    try
                    {
                        if (el.TryGetProperty("content", out var contentElement))
                        {
                            System.Diagnostics.Debug.WriteLine("[DOTNET] Structure du content:");
                            System.Diagnostics.Debug.WriteLine(contentElement.ToString());
                        }
                        
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
                            System.Diagnostics.Debug.WriteLine("[DOTNET] Content trouvé, tentative de conversion...");
                            book.Content = ReadByteArray(contentsElement);
                            System.Diagnostics.Debug.WriteLine($"[DOTNET] Taille du content : {book.Content?.Length ?? 0} bytes");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("[DOTNET] Pas de champ 'content' ou content est null !");
                        }

                        books.Add(book);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DOTNET] Erreur lors du parsing d'un livre : {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[DOTNET] StackTrace: {ex.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DOTNET] Erreur globale de parsing : {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[DOTNET] StackTrace: {ex.StackTrace}");
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
                Console.WriteLine($"[DOTNET] Total des livres chargés : {books.Count}");
                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DOTNET] Erreur lors du chargement des livres : {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des livres par nom : {ex.Message}");
                return new List<Book>();
            }
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/book/{id}");
                var data = await response.Content.ReadAsStringAsync();

                // Désérialise directement le livre (pas la liste)
                using JsonDocument doc = JsonDocument.Parse(data);
                var root = doc.RootElement;

                // Si la racine est un objet livre unique
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
                        System.Diagnostics.Debug.WriteLine("[DOTNET] Content trouvé, tentative de conversion (livre unique)...");
                        book.Content = ReadByteArray(contentsElement);
                        System.Diagnostics.Debug.WriteLine($"[DOTNET] Taille du content : {book.Content?.Length ?? 0} bytes");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[DOTNET] Pas de champ 'content' dans la réponse !");
                    }

                    return book;
                }
                else
                {
                    // fallback: ancienne logique (liste)
                    var books = ParseBooks(data);
                    return books.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du livre par ID : {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des livres par date : {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des livres par utilisateur : {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création du livre : {ex.Message}");
            }
        }
    }
}
