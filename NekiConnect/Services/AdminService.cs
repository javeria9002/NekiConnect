using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Interfaces;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class AdminService : IAdminService  
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public AdminService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        // ── NGO approvals ──────────────────────────────────────────
        public async Task<List<NGO>> GetAllNGOsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NGOs.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<List<NGO>> GetNGOsByStatusAsync(string status)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NGOs.Where(n => n.Status == status)
                                .OrderByDescending(n => n.CreatedAt)
                                .ToListAsync();
        }

        public async Task ApproveNGOAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var ngo = await db.NGOs.FindAsync(ngoId);
            if (ngo is null) return;
            ngo.Status = "Approved";
            ngo.RejectionReason = null;
            await db.SaveChangesAsync();
        }

        public async Task RejectNGOAsync(int ngoId, string reason)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var ngo = await db.NGOs.FindAsync(ngoId);
            if (ngo is null) return;
            ngo.Status = "Rejected";
            ngo.RejectionReason = reason;
            await db.SaveChangesAsync();
        }

        public async Task SuspendNGOAsync(int ngoId, string reason)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var ngo = await db.NGOs.FindAsync(ngoId);
            if (ngo is null) return;
            ngo.Status = "Suspended";
            ngo.SuspensionReason = reason;  // ✅ save reason
            await db.SaveChangesAsync();
        }

        public async Task DeleteNGOAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var ngo = await db.NGOs.FindAsync(ngoId);
            if (ngo is null) return;

            // remove the NGO's campaigns first (avoids FK restrict)
            var campaigns = await db.Campaigns.Where(c => c.NgoId == ngoId).ToListAsync();
            db.Campaigns.RemoveRange(campaigns);
            db.NGOs.Remove(ngo);
            await db.SaveChangesAsync();
        }

        // ── Stats ──────────────────────────────────────────────────
        public async Task<int> GetTotalNGOsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NGOs.CountAsync();
        }

        public async Task<int> GetPendingNGOsCountAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.NGOs.CountAsync(n => n.Status == "Pending");
        }

        public async Task<int> GetTotalUsersAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Users.CountAsync();
        }

        public async Task<int> GetActiveCampaignsCountAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Campaigns.CountAsync(c => c.Status == "Upcoming");
        }

        public async Task<decimal> GetTotalDonationsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Donations.SumAsync(d => (decimal?)d.Amount) ?? 0m;
        }

        // Last 6 months of platform-wide donations (bar chart)
        public async Task<List<MonthlyDonation>> GetPlatformMonthlyAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);

            var rows = await db.Donations
                .Where(d => d.DonatedAt >= start)
                .Select(d => new { d.DonatedAt, d.Amount })
                .ToListAsync();

            var result = new List<MonthlyDonation>();
            for (int i = 0; i < 6; i++)
            {
                var m = start.AddMonths(i);
                var total = rows.Where(r => r.DonatedAt.Year == m.Year && r.DonatedAt.Month == m.Month)
                                .Sum(r => r.Amount);
                result.Add(new MonthlyDonation { Month = m.ToString("MMM"), Total = total });
            }
            return result;
        }

        // Donations grouped by the donating campaign's NGO category (donut chart)
        public async Task<List<CategoryTotal>> GetDonationsByCategoryAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            var rows = await db.Donations
                .Where(d => d.CampaignId != null && d.Campaign!.NGO != null)
                .Select(d => new { Category = d.Campaign!.NGO!.Category, d.Amount })
                .ToListAsync();

            return rows
                .GroupBy(r => string.IsNullOrWhiteSpace(r.Category) ? "Other" : r.Category)
                .Select(g => new CategoryTotal { Category = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(c => c.Total)
                .ToList();
        }

        // Users with their total donated + whether they've volunteered
        public async Task<List<UserOverview>> GetUserOverviewAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            var users = await db.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

            var donationSums = await db.Donations
                .GroupBy(d => d.DonorId)
                .Select(g => new { DonorId = g.Key, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var volunteerIds = await db.VolunteerApplications
                .Select(v => v.UserId)
                .Distinct()
                .ToListAsync();

            return users.Select(u => new UserOverview
            {
                User = u,
                TotalDonated = donationSums.FirstOrDefault(d => d.DonorId == u.Id)?.Total ?? 0m,
                HasVolunteered = volunteerIds.Contains(u.Id)
            }).ToList();
        }

        public async Task<List<Donation>> GetAllDonationsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Donations
                .Include(d => d.Donor)
                .Include(d => d.Campaign).ThenInclude(c => c!.NGO)
                .OrderByDescending(d => d.DonatedAt)
                .ToListAsync();
        }

        // NGO directory with campaign count, total raised, volunteers
        public async Task<List<NgoOverview>> GetNgoOverviewAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            var ngos = await db.NGOs.OrderByDescending(n => n.CreatedAt).ToListAsync();

            var campaigns = await db.Campaigns
                .Select(c => new { c.NgoId, c.RaisedAmount })
                .ToListAsync();

            var volunteers = await db.VolunteerApplications
                .Where(v => v.Campaign != null)
                .Select(v => new { NgoId = v.Campaign!.NgoId })
                .ToListAsync();

            var now = DateTime.Today;
            return ngos.Select(n =>
            {
                var ngoCampaigns = campaigns.Where(c => c.NgoId == n.Id).ToList();
                return new NgoOverview
                {
                    NGO = n,
                    Campaigns = ngoCampaigns.Count,
                    TotalRaised = ngoCampaigns.Sum(c => c.RaisedAmount),
                    Volunteers = volunteers.Count(v => v.NgoId == n.Id),
                    IsNew = n.CreatedAt.Year == now.Year && n.CreatedAt.Month == now.Month
                };
            }).ToList();
        }
    }
}