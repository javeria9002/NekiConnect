using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class NGOService : INGOService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public NGOService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        // ── Existing methods ──────────────────────────────────────

        public async Task RegisterNGOAsync(NGO ngo)
        {
            await using var db = await _factory.CreateDbContextAsync();
            ngo.Status = "Pending";
            ngo.CreatedAt = DateTime.UtcNow;
            db.NGOs.Add(ngo);
            await db.SaveChangesAsync();
        }

        public async Task<NGO?> GetNGOByUserIdAsync(string userId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NGOs.FirstOrDefaultAsync(n => n.UserId == userId);
        }

        public async Task<List<NGO>> GetAllApprovedNGOsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NGOs
                .Where(n => n.Status == "Approved")
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<NGO?> GetNGOByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NGOs.FindAsync(id);
        }

        public async Task UpdateAsync(NGO ngo)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.NGOs.Update(ngo);
            await db.SaveChangesAsync();
        }

        // ── Dashboard stats ────────────────────────────────────────

        public async Task<decimal> GetTotalDonationsAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Campaigns
                .Where(c => c.NgoId == ngoId)
                .SumAsync(c => (decimal?)c.RaisedAmount) ?? 0m;
        }

        public async Task<int> GetActiveCampaignsCountAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Campaigns
                .CountAsync(c => c.NgoId == ngoId && c.Status == "Upcoming");
        }

        public async Task<int> GetUpcomingEventsCountAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Events
                .CountAsync(e => e.NgoId == ngoId
                              && e.Status == "Upcoming"
                              && e.EventDate >= DateTime.Today);
        }

        // ✅ Fixed — counts both campaign AND event pending applications
        public async Task<int> GetPendingVolunteersCountAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.VolunteerApplications
                .Include(v => v.Campaign)
                .Include(v => v.Event)
                .CountAsync(v => v.Status == "Pending" &&
                                ((v.Campaign != null && v.Campaign.NgoId == ngoId) ||
                                 (v.Event != null && v.Event.NgoId == ngoId)));
        }

        public async Task<Dictionary<int, double>> GetAverageRatingsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Feedbacks
                .GroupBy(f => f.NgoId)
                .Select(g => new { NgoId = g.Key, Avg = g.Average(f => f.Rating) })
                .ToDictionaryAsync(x => x.NgoId, x => x.Avg);
        }

        public async Task<Dictionary<int, int>> GetRatingCountsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Feedbacks
                .GroupBy(f => f.NgoId)
                .Select(g => new { NgoId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.NgoId, x => x.Count);
        }

        public async Task<List<MonthlyDonation>> GetMonthlyDonationsAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);

            var rows = await db.Donations
                .Where(d => d.CampaignId != null
                         && d.Campaign!.NgoId == ngoId
                         && d.DonatedAt >= start)
                .Select(d => new { d.DonatedAt, d.Amount })
                .ToListAsync();

            var result = new List<MonthlyDonation>();
            for (int i = 0; i < 6; i++)
            {
                var m = start.AddMonths(i);
                var total = rows.Where(r => r.DonatedAt.Year == m.Year
                                         && r.DonatedAt.Month == m.Month)
                                .Sum(r => r.Amount);
                result.Add(new MonthlyDonation { Month = m.ToString("MMM"), Total = total });
            }
            return result;
        }
    }
}