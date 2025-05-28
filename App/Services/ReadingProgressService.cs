using App.Database;
using App.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Services
{
    public class ReadingProgressService
    {
        public async Task SaveProgressAsync(int bookId, int page, string chapter, double position)
        {
            using var context = new DataContext();

            var progress = await context.ReadingProgress
                .AsNoTracking() // <-- Ajoute AsNoTracking pour éviter les conflits de tracking
                .FirstOrDefaultAsync(p => p.BookId == bookId);

            if (progress == null)
            {
                progress = new ReadingProgress { BookId = bookId };
                progress.Page = page;
                progress.Chapter = chapter;
                progress.Position = position;
                progress.LastRead = DateTime.UtcNow;
                context.ReadingProgress.Add(progress);
            }
            else
            {
                // Crée un nouvel objet pour l'update
                var updated = new ReadingProgress
                {
                    Id = progress.Id,
                    BookId = bookId,
                    Page = page,
                    Chapter = chapter,
                    Position = position,
                    LastRead = DateTime.UtcNow
                };
                context.ReadingProgress.Update(updated);
            }

            await context.SaveChangesAsync();
        }

        public async Task<ReadingProgress> GetProgressAsync(int bookId)
        {
            using var context = new DataContext();
            return await context.ReadingProgress
                .FirstOrDefaultAsync(p => p.BookId == bookId);
        }
    }
}
