using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Fetch a single user by their Identity ID
        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _db.Users.FindAsync(id);
        }

        // Admin — full list of all registered users
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _db.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        // User updates their own profile (name only — email/password go through Identity)
        public async Task UpdateUserAsync(ApplicationUser updated)
        {
            var user = await _db.Users.FindAsync(updated.Id);
            if (user is null) return;

            user.FullName = updated.FullName;
            await _db.SaveChangesAsync();
        }

        // Admin suspends a user — they can no longer log in
        public async Task SuspendUserAsync(string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null) return;

            user.IsSuspended = true;
            await _db.SaveChangesAsync();
        }

        // Admin re-activates a suspended user
        public async Task UnsuspendUserAsync(string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null) return;

            user.IsSuspended = false;
            await _db.SaveChangesAsync();
        }

        // Admin permanently removes a user
        public async Task DeleteUserAsync(string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null) return;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}