using System;
using System.IO;
using App.Models;
using App.Services;
using Microsoft.EntityFrameworkCore;


namespace App.Database
{
    public class DataContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReadingProgress> ReadingProgress { get; set; }
        private readonly BookApi _bookApi = new();
        public string DbPath { get; }

        public DataContext()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DbPath = Path.Combine(folder, "passion_lecture.db");
            EnsureDatabaseCreated();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        public void EnsureDatabaseCreated()
        {
            // Supprimez la ligne suivante :
            // this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }

        public async Task SyncBookAsync()
        {
            try
            {
                var books = await _bookApi.GetAllBooksAsync();
                
                // Clear database first
                await Database.ExecuteSqlRawAsync("DELETE FROM Books");
                await Database.ExecuteSqlRawAsync("DELETE FROM SQLite_sequence WHERE name='Books'");
                
                foreach (var book in books)
                {
                    try
                    {
                        // Create new entry
                        var newBook = new Book
                        {
                            Id = book.Id,
                            Name = book.Name,
                            Summary = book.Summary,
                            Passage = book.Passage,
                            EditionYear = book.EditionYear,
                            Pages = book.Pages,
                            Content = book.Content,
                            CoverImage = book.CoverImage ?? Array.Empty<byte>(),
                            Created = book.Created,
                            CategoryFk = book.CategoryFk,
                            EditorFk = book.EditorFk,
                            AuthorFk = book.AuthorFk
                        };

                        await Books.AddAsync(newBook);
                        await SaveChangesAsync();
                        System.Diagnostics.Debug.WriteLine($"[SUCCESS] Book {book.Id} saved successfully");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to save book {book.Id}: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[ERROR] Inner exception: {ex.InnerException?.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Global sync error: {ex.Message}");
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(e => e.Id);

                // Make all FK nullable
                entity.Property(e => e.CategoryFk).IsRequired(false);
                entity.Property(e => e.EditorFk).IsRequired(false);
                entity.Property(e => e.AuthorFk).IsRequired(false);

                // Allow large content
                entity.Property(e => e.Content).HasColumnType("BLOB").IsRequired(false);
                entity.Property(e => e.CoverImage).HasColumnType("BLOB").IsRequired(false); // <-- Ajoute IsRequired(false)
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });

            // Configure the many-to-many relationship with a join table
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookTags",
                    j => j
                        .HasOne<Tag>()
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<ReadingProgress>(entity =>
            {
                entity.ToTable("ReadingProgress");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Book)
                      .WithMany()
                      .HasForeignKey(e => e.BookId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Ajouter des tags de test
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Fantasy" },
                new Tag { Id = 2, Name = "Science-Fiction" },
                new Tag { Id = 3, Name = "Romance" },
                new Tag { Id = 4, Name = "Thriller" },
                new Tag { Id = 5, Name = "Historique" }
            );
        }
    }
}