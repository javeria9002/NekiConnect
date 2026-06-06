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

        // ── NGO: all applications (campaign + event) ──
        public async Task<List<VolunteerApplication>> GetApplicationsByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.VolunteerApplications
                .Include(v => v.User)
                .Include(v => v.Campaign)
                .Include(v => v.Event)
                .Where(v => (v.Campaign != null && v.Campaign.NgoId == ngoId) ||
                            (v.Event != null && v.Event.NgoId == ngoId))
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        // ── User: my applications ──
        public async Task<List<VolunteerApplication>> GetApplicationsByUserIdAsync(string userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.VolunteerApplications
                .Include(v => v.Campaign)
                    .ThenInclude(c => c!.NGO)
                .Include(v => v.Event)
                    .ThenInclude(e => e!.NGO)
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        // ── Campaign: applications ──
        public async Task<List<VolunteerApplication>> GetApplicationsByCampaignIdAsync(int campaignId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.VolunteerApplications
                .Include(v => v.User)
                .Where(v => v.CampaignId == campaignId)
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        // ── Event: applications ──
        public async Task<List<VolunteerApplication>> GetApplicationsByEventIdAsync(int eventId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.VolunteerApplications
                .Include(v => v.User)
                .Where(v => v.EventId == eventId)
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        // ── Check duplicate campaign application ──
        public async Task<bool> HasAppliedAsync(string userId, int campaignId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.VolunteerApplications
                .AnyAsync(v => v.UserId == userId && v.CampaignId == campaignId);
        }

        // ── Check duplicate event application ──
        public async Task<bool> HasAppliedToEventAsync(string userId, int eventId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.VolunteerApplications
                .AnyAsync(v => v.UserId == userId && v.EventId == eventId);
        }

        // ── Apply for campaign or event ──
        public async Task ApplyAsync(VolunteerApplication application)
        {
            await using var db = await _factory.CreateDbContextAsync();

            application.Status = "Pending";
            application.AppliedAt = DateTime.UtcNow;

            db.VolunteerApplications.Add(application);
            await db.SaveChangesAsync();
        }

        // ── Update status (approve/reject) ──
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

        // ── Count applications (user dashboard) ──
        public async Task<int> GetApplicationsCountAsync(string userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.VolunteerApplications
                .CountAsync(v => v.UserId == userId);
        }
    }
}
