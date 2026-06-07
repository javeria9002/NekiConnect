using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class BranchService : IBranchService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public BranchService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<NgoBranch>> GetByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NgoBranches
                .Where(b => b.NgoId == ngoId)
                .OrderBy(b => b.BranchName)
                .ToListAsync();
        }

        public async Task CreateAsync(NgoBranch branch)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.NgoBranches.Add(branch);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var b = await db.NgoBranches.FindAsync(id);
            if (b != null)
            {
                db.NgoBranches.Remove(b);
                await db.SaveChangesAsync();
            }
        }
    }
}