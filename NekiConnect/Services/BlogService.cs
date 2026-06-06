using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class BlogService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public BlogService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        // ── NGO specific blogs ──
        public async Task<List<BlogPost>> GetByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.BlogPosts
                .Where(b => b.NgoId == ngoId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        // ── Admin: all blogs ──
        public async Task<List<BlogPost>> GetAllBlogsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.BlogPosts
                .Include(b => b.NGO)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        // ── Public published blogs ──
        public async Task<List<BlogPost>> GetAllPublishedAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.BlogPosts
                .Include(b => b.NGO)
                .Where(b => b.Status == "Published")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        // ── Single blog ──
        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.BlogPosts
                .Include(b => b.NGO)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // ── Create ──
        public async Task CreateAsync(BlogPost post)
        {
            await using var db = await _factory.CreateDbContextAsync();

            post.CreatedAt = DateTime.UtcNow;

            db.BlogPosts.Add(post);
            await db.SaveChangesAsync();
        }

        // ── Update ──
        public async Task UpdateAsync(BlogPost post)
        {
            await using var db = await _factory.CreateDbContextAsync();

            db.BlogPosts.Update(post);
            await db.SaveChangesAsync();
        }

        // ── Delete ──
        public async Task DeleteAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var post = await db.BlogPosts.FindAsync(id);
            if (post != null)
            {
                db.BlogPosts.Remove(post);
                await db.SaveChangesAsync();
            }
        }

        // ── Stats ──
        public async Task<int> GetCountAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.BlogPosts.CountAsync(b => b.NgoId == ngoId);
        }
    }
}