using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class NGOService
    {
        private readonly ApplicationDbContext _db;

        public NGOService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Called when an NGO submits their registration form
        public async Task RegisterNGOAsync(NGO ngo)
        {
            ngo.Status = "Pending";
            ngo.CreatedAt = DateTime.UtcNow;
            _db.NGOs.Add(ngo);
            await _db.SaveChangesAsync();
        }

        // Get NGO by their owner's User ID
        public async Task<NGO?> GetNGOByUserIdAsync(string userId)
        {
            return await _db.NGOs.FirstOrDefaultAsync(n => n.UserId == userId);
        }

        // Get all approved NGOs (for homepage)
        public async Task<List<NGO>> GetAllApprovedNGOsAsync()
        {
            return await _db.NGOs
                .Where(n => n.Status == "Approved")
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // Get NGO by ID (for public profile)
        public async Task<NGO?> GetNGOByIdAsync(int id)
        {
            return await _db.NGOs.FindAsync(id);
        }
    }
}