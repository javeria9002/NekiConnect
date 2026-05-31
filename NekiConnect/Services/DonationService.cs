using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class DonationService
    {
        private readonly ApplicationDbContext _db;

        public DonationService(ApplicationDbContext db) { _db = db; }

        public async Task<List<Donation>> GetDonationsByDonorIdAsync(string donorId)
        {
            return await _db.Donations
                .Include(d => d.Campaign).ThenInclude(c => c!.NGO)
                .Where(d => d.DonorId == donorId)
                .OrderByDescending(d => d.DonatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalDonatedAsync(string donorId)
        {
            return await _db.Donations
                .Where(d => d.DonorId == donorId)
                .SumAsync(d => d.Amount);
        }

        public async Task<decimal> GetThisMonthDonatedAsync(string donorId)
        {
            var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return await _db.Donations
                .Where(d => d.DonorId == donorId && d.DonatedAt >= start)
                .SumAsync(d => d.Amount);
        }

        public async Task<int> GetCampaignsSupportedCountAsync(string donorId)
        {
            return await _db.Donations
                .Where(d => d.DonorId == donorId && d.CampaignId != null)
                .Select(d => d.CampaignId)
                .Distinct()
                .CountAsync();
        }

        public async Task<Donation> AddDonationAsync(Donation donation)
        {
            _db.Donations.Add(donation);

            if (donation.CampaignId.HasValue)
            {
                var campaign = await _db.Campaigns.FindAsync(donation.CampaignId.Value);
                if (campaign != null)
                    campaign.RaisedAmount += donation.Amount;
            }

            await _db.SaveChangesAsync();
            return donation;
        }
    }
}