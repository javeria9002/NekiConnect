using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class VolunteerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public VolunteerService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        // All applications for one NGO's campaigns (with applicant + campaign)
        public async Task<List<VolunteerApplication>> GetApplicationsByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.VolunteerApplications
                .Include(v => v.User)
                .Include(v => v.Campaign)
                .Where(v => v.Campaign != null && v.Campaign.NgoId == ngoId)
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        public async Task<List<VolunteerApplication>> GetApplicationsByUserIdAsync(string userId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.VolunteerApplications
                .Include(v => v.Campaign).ThenInclude(c => c!.NGO)
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        public async Task<List<VolunteerApplication>> GetApplicationsByCampaignIdAsync(int campaignId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.VolunteerApplications
                .Include(v => v.User)
                .Where(v => v.CampaignId == campaignId)
                .ToListAsync();
        }

        public async Task<bool> HasAppliedAsync(string userId, int campaignId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.VolunteerApplications
                .AnyAsync(v => v.UserId == userId && v.CampaignId == campaignId);
        }

        public async Task ApplyAsync(VolunteerApplication application)
        {
            await using var db = await _factory.CreateDbContextAsync();
            application.Status = "Pending";
            application.AppliedAt = DateTime.UtcNow;
            db.VolunteerApplications.Add(application);
            await db.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int applicationId, string status)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var app = await db.VolunteerApplications.FindAsync(applicationId);
            if (app != null)
            {
                app.Status = status;
                await db.SaveChangesAsync();
            }
        }

        public async Task<int> GetApplicationsCountAsync(string userId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.VolunteerApplications.CountAsync(v => v.UserId == userId);
        }
    }
}