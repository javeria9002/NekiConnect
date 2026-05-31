using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class VolunteerService
    {
        private readonly ApplicationDbContext _db;

        public VolunteerService(ApplicationDbContext db) { _db = db; }

        public async Task<List<VolunteerApplication>> GetApplicationsByUserIdAsync(string userId)
        {
            return await _db.VolunteerApplications
                .Include(v => v.Campaign).ThenInclude(c => c!.NGO)
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        public async Task<List<VolunteerApplication>> GetApplicationsByCampaignIdAsync(int campaignId)
        {
            return await _db.VolunteerApplications
                .Include(v => v.User)
                .Where(v => v.CampaignId == campaignId)
                .OrderByDescending(v => v.AppliedAt)
                .ToListAsync();
        }

        public async Task<bool> HasAppliedAsync(string userId, int campaignId)
        {
            return await _db.VolunteerApplications
                .AnyAsync(v => v.UserId == userId && v.CampaignId == campaignId);
        }

        public async Task<VolunteerApplication> ApplyAsync(VolunteerApplication application)
        {
            _db.VolunteerApplications.Add(application);
            await _db.SaveChangesAsync();
            return application;
        }

        public async Task UpdateStatusAsync(int applicationId, string status)
        {
            var app = await _db.VolunteerApplications.FindAsync(applicationId);
            if (app != null)
            {
                app.Status = status;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<int> GetApplicationsCountAsync(string userId)
        {
            return await _db.VolunteerApplications
                .CountAsync(v => v.UserId == userId);
        }
    }
}