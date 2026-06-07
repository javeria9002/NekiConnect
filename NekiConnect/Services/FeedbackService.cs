using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Interfaces;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public FeedbackService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Feedback>> GetByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Feedbacks
                .Include(f => f.User)
                .Where(f => f.NgoId == ngoId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasReviewedAsync(string userId, int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Feedbacks
                .AnyAsync(f => f.UserId == userId && f.NgoId == ngoId);
        }

        // ✅ Has user donated to any campaign of this NGO?
        public async Task<bool> IsVerifiedDonorAsync(string userId, int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Donations
                .AnyAsync(d => d.DonorId == userId
                            && d.CampaignId != null
                            && d.Campaign!.NgoId == ngoId);
        }

        // ✅ Has user volunteered with any campaign/event of this NGO?
        public async Task<bool> IsVerifiedVolunteerAsync(string userId, int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.VolunteerApplications
                .AnyAsync(v => v.UserId == userId
                            && v.Status == "Accepted"
                            && ((v.CampaignId != null && v.Campaign!.NgoId == ngoId)
                             || (v.EventId != null && v.Event!.NgoId == ngoId)));
        }

        // ✅ Returns "Donor", "Volunteer", or null
        public async Task<string?> GetVerifiedTypeAsync(string userId, int ngoId)
        {
            if (await IsVerifiedDonorAsync(userId, ngoId)) return "Donor";
            if (await IsVerifiedVolunteerAsync(userId, ngoId)) return "Volunteer";
            return null;
        }

        public async Task AddAsync(Feedback feedback)
        {
            await using var db = await _factory.CreateDbContextAsync();
            feedback.CreatedAt = DateTime.UtcNow;
            db.Feedbacks.Add(feedback);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var f = await db.Feedbacks.FindAsync(id);
            if (f != null)
            {
                db.Feedbacks.Remove(f);
                await db.SaveChangesAsync();
            }
        }
    }
}