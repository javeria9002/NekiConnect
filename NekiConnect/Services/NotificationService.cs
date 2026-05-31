using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _db;

        public NotificationService(ApplicationDbContext db) { _db = db; }

        public async Task<List<Notification>> GetByUserIdAsync(string userId)
        {
            return await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _db.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllReadAsync(string userId)
        {
            var notifications = await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
            notifications.ForEach(n => n.IsRead = true);
            await _db.SaveChangesAsync();
        }

        public async Task CreateAsync(string userId, string title, string message, string type = "Info", string? link = null)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                LinkUrl = link
            });
            await _db.SaveChangesAsync();
        }

        public async Task NotifyDonationReceiptAsync(string donorId, decimal amount, string campaignTitle)
        {
            await CreateAsync(donorId,
                "Donation Confirmed",
                $"Thank you! Your donation of PKR {amount:N0} to '{campaignTitle}' was received.",
                "Donation", "/donor/donations");
        }

        public async Task NotifyVolunteerStatusAsync(string userId, string campaignTitle, bool accepted)
        {
            var status = accepted ? "accepted" : "rejected";
            await CreateAsync(userId,
                accepted ? "Application Accepted!" : "Application Update",
                $"Your volunteer application for '{campaignTitle}' was {status}.",
                accepted ? "Success" : "Info", "/donor/applications");
        }

        public async Task NotifyNGOStatusAsync(string userId, bool approved, string? reason = null)
        {
            await CreateAsync(userId,
                approved ? "NGO Approved!" : "NGO Application Update",
                approved ? "Your NGO has been approved. You can now access your NGO dashboard."
                         : $"Your NGO was not approved. Reason: {reason}",
                approved ? "Success" : "Warning", "/ngo/dashboard");
        }
    }
}