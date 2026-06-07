using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Interfaces;


namespace NekiConnect.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public BeneficiaryService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Beneficiary>> GetByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Beneficiaries
                .Where(b => b.NgoId == ngoId)
                .OrderByDescending(b => b.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Beneficiary?> GetByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Beneficiaries.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task CreateAsync(Beneficiary b)
        {
            await using var db = await _factory.CreateDbContextAsync();
            b.CreatedAt = DateTime.UtcNow;
            b.UpdatedAt = DateTime.UtcNow;
            db.Beneficiaries.Add(b);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Beneficiary b)
        {
            await using var db = await _factory.CreateDbContextAsync();
            b.UpdatedAt = DateTime.UtcNow;
            db.Beneficiaries.Update(b);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var b = await db.Beneficiaries.FindAsync(id);
            if (b != null)
            {
                db.Beneficiaries.Remove(b);
                await db.SaveChangesAsync();
            }
        }
    }
}