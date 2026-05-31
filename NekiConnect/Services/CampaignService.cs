using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class CampaignService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public CampaignService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Campaign>> GetAllActiveCampaignsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Campaigns
                .Include(c => c.NGO)
                .Where(c => c.Status == "Upcoming")
                .OrderBy(c => c.CampaignDate)
                .ToListAsync();
        }

        public async Task<Campaign?> GetCampaignByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Campaigns
                .Include(c => c.NGO)
                .Include(c => c.Donations)
                .Include(c => c.VolunteerApplications)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Campaign>> GetCampaignsByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Campaigns
                .Include(c => c.Donations)
                .Include(c => c.VolunteerApplications)
                .Where(c => c.NgoId == ngoId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateCampaignAsync(Campaign campaign)
        {
            await using var db = await _factory.CreateDbContextAsync();
            campaign.CreatedAt = DateTime.UtcNow;
            if (string.IsNullOrEmpty(campaign.Status))
                campaign.Status = "Upcoming";
            db.Campaigns.Add(campaign);
            await db.SaveChangesAsync();
        }

        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Campaigns.Update(campaign);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var c = await db.Campaigns.FindAsync(id);
            if (c != null)
            {
                db.Campaigns.Remove(c);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateRaisedAmountAsync(int campaignId, decimal amount)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var c = await db.Campaigns.FindAsync(campaignId);
            if (c != null)
            {
                c.RaisedAmount += amount;
                await db.SaveChangesAsync();
            }
        }
    }
}