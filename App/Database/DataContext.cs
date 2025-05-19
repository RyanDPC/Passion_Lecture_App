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
            // Détermine le chemin du fichier de base de données SQLite
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DbPath = Path.Combine(folder, "passion_lecture.db");
            EnsureDatabaseCreated();
            // Insérer les données books
            SyncBookAsync().Wait();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        public void EnsureDatabaseCreated()
        {
            // Crée la base de données et les tables si elles n'existent pas
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
            await using var context = new DataContext();

            // Ajoute ou met à jour les livres dans la base de données
            foreach (var book in books)
            {
                if (await context.Books.AnyAsync(b => b.Id == book.Id))
                {
                    context.Books.Update(book);  // Si le livre existe déjà, on le met à jour
                }
                else
                {
                    context.Books.Add(book);  // Sinon, on l'ajoute
                }
            }

            await context.SaveChangesAsync();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuration de la relation Book-User
            modelBuilder.Entity<Book>()
                .HasOne(d => d.User)
                .WithMany(u => u.Books)
                .HasForeignKey(d => d.UserFk)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuration de la propriété CoverImage
            modelBuilder.Entity<Book>()
                .Property(d => d.CoverImage)
                .IsRequired(false);
                
            // Configuration de la relation Book-Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BookFk);
                
            // Configuration de la relation Comment-User
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserFk);
                
            // Configuration de la relation Book-Tag (many-to-many)
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.Books)
                .UsingEntity(j => j.ToTable("BookTags"));
        }
    }
} 