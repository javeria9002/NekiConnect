using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public UserService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Users.FindAsync(id);
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateUserAsync(ApplicationUser updated)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var user = await db.Users.FindAsync(updated.Id);
            if (user is null) return;

            user.FullName = updated.FullName;
            await db.SaveChangesAsync();
        }

        public async Task SuspendUserAsync(string id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var user = await db.Users.FindAsync(id);
            if (user is null) return;

            user.IsSuspended = true;
            await db.SaveChangesAsync();
        }

        public async Task UnsuspendUserAsync(string id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var user = await db.Users.FindAsync(id);
            if (user is null) return;

            user.IsSuspended = false;
            await db.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var user = await db.Users.FindAsync(id);
            if (user is null) return;

            db.Users.Remove(user);
            await db.SaveChangesAsync();
        }
    }
}