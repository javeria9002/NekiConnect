using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public DonationService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Donation>> GetDonationsByDonorIdAsync(string donorId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Donations
                .Include(d => d.Campaign).ThenInclude(c => c!.NGO)
                .Where(d => d.DonorId == donorId)
                .OrderByDescending(d => d.DonatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalDonatedAsync(string donorId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Donations
                .Where(d => d.DonorId == donorId)
                .SumAsync(d => d.Amount);
        }

        public async Task<decimal> GetThisMonthDonatedAsync(string donorId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return await db.Donations
                .Where(d => d.DonorId == donorId && d.DonatedAt >= start)
                .SumAsync(d => d.Amount);
        }

        public async Task<int> GetCampaignsSupportedCountAsync(string donorId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Donations
                .Where(d => d.DonorId == donorId && d.CampaignId != null)
                .Select(d => d.CampaignId)
                .Distinct()
                .CountAsync();
        }

        public async Task<Donation> AddDonationAsync(Donation donation)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Donations.Add(donation);

            if (donation.CampaignId.HasValue)
            {
                var campaign = await db.Campaigns.FindAsync(donation.CampaignId.Value);
                if (campaign != null)
                    campaign.RaisedAmount += donation.Amount;
            }

            await db.SaveChangesAsync();
            return donation;
        }
    }
}