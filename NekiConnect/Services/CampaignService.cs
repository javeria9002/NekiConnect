using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class CampaignService
    {
        private readonly ApplicationDbContext _db;

        public CampaignService(ApplicationDbContext db) { _db = db; }

        public async Task<List<Campaign>> GetAllActiveCampaignsAsync()
        {
            return await _db.Campaigns
                .Include(c => c.NGO)
                .Where(c => c.Status == "Upcoming")
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Campaign?> GetCampaignByIdAsync(int id)
        {
            return await _db.Campaigns
                .Include(c => c.NGO)
                .Include(c => c.Donations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Campaign>> GetCampaignsByNgoIdAsync(int ngoId)
        {
            return await _db.Campaigns
                .Where(c => c.NgoId == ngoId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            _db.Campaigns.Add(campaign);
            await _db.SaveChangesAsync();
            return campaign;
        }

        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            _db.Campaigns.Update(campaign);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(int id)
        {
            var campaign = await _db.Campaigns.FindAsync(id);
            if (campaign != null)
            {
                _db.Campaigns.Remove(campaign);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateRaisedAmountAsync(int campaignId, decimal amount)
        {
            var campaign = await _db.Campaigns.FindAsync(campaignId);
            if (campaign != null)
            {
                campaign.RaisedAmount += amount;
                await _db.SaveChangesAsync();
            }
        }
    }
}