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
            this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }

        public async Task SyncBookAsync()
        {
            var books = await _bookApi.GetAllBooksAsync();
            await InsertAllBooksAsync(books);
        }

        public async Task InsertAllBooksAsync(List<Book> books)
        {
            foreach (var book in books)
            {
                var existing = await Books.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == book.Id);
                if (existing != null)
                {
                    // Mise à jour propriété par propriété
                    existing.Name = book.Name;
                    existing.Passage = book.Passage;
                    existing.Summary = book.Summary;
                    existing.EditionYear = book.EditionYear;
                    existing.CoverImage = book.CoverImage;
                    existing.Pages = book.Pages;

                    Books.Update(existing);
                }
                else
                {
                    Books.Add(book);
                }
            }
            try
            {
                await Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");
                await SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Erreur lors de SaveChangesAsync : " + ex.InnerException?.Message ?? ex.Message);
                throw;
            }

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(d => d.CoverImage)
                .IsRequired(false);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BookFk);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.Books)
                .UsingEntity(j => j.ToTable("BookTags"));
        }
    }
} 