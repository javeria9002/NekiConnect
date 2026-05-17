using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class BlogService
    {
        private readonly ApplicationDbContext _db;

        public BlogService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<BlogPost>> GetAllBlogsAsync()
        {
            return await _db.BlogPosts
                .Include(b => b.NGO)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetBlogsByNgoIdAsync(int ngoId)
        {
            return await _db.BlogPosts
                .Where(b => b.NgoId == ngoId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetBlogByIdAsync(int id)
        {
            return await _db.BlogPosts
                .Include(b => b.NGO)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task CreateBlogAsync(BlogPost blog)
        {
            blog.CreatedAt = DateTime.UtcNow;
            _db.BlogPosts.Add(blog);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateBlogAsync(BlogPost updated)
        {
            var blog = await _db.BlogPosts.FindAsync(updated.Id);
            if (blog is null) return;

            blog.Title = updated.Title;
            blog.Content = updated.Content;
            blog.ImageUrl = updated.ImageUrl;
            blog.Category = updated.Category; // ── ADDED ──

            await _db.SaveChangesAsync();
        }

        public async Task DeleteBlogAsync(int id)
        {
            var blog = await _db.BlogPosts.FindAsync(id);
            if (blog is null) return;

            _db.BlogPosts.Remove(blog);
            await _db.SaveChangesAsync();
        }
    }
}