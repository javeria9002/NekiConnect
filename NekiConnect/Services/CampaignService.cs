using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public CampaignService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        // ── All active/upcoming campaigns ──
        public async Task<List<Campaign>> GetAllActiveCampaignsAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Campaigns
                .Include(c => c.NGO)
                .Where(c => c.Status == "Upcoming")
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // ── Single campaign ──
        public async Task<Campaign?> GetCampaignByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Campaigns
                .Include(c => c.NGO)
                .Include(c => c.Donations)
                .Include(c => c.VolunteerApplications)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // ── NGO campaigns ──
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

        // ── Create campaign ──
        public async Task CreateCampaignAsync(Campaign campaign)
        {
            await using var db = await _factory.CreateDbContextAsync();

            campaign.CreatedAt = DateTime.UtcNow;

            if (string.IsNullOrEmpty(campaign.Status))
                campaign.Status = "Upcoming";

            db.Campaigns.Add(campaign);
            await db.SaveChangesAsync();
        }

        // ── Update campaign ──
        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            await using var db = await _factory.CreateDbContextAsync();

            db.Campaigns.Update(campaign);
            await db.SaveChangesAsync();
        }

        // ── Delete campaign ──
        public async Task DeleteCampaignAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var campaign = await db.Campaigns.FindAsync(id);

            if (campaign != null)
            {
                db.Campaigns.Remove(campaign);
                await db.SaveChangesAsync();
            }
        }

        // ── Donation updates ──
        public async Task UpdateRaisedAmountAsync(int campaignId, decimal amount)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var campaign = await db.Campaigns.FindAsync(campaignId);

            if (campaign != null)
            {
                campaign.RaisedAmount += amount;
                await db.SaveChangesAsync();
            }
        }
    }
}