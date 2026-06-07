using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetByUserIdAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAllReadAsync(string userId);
        Task CreateAsync(string userId, string title, string message, string type = "Info", string? link = null);
        Task NotifyDonationReceiptAsync(string donorId, decimal amount, string campaignTitle);
        Task NotifyVolunteerStatusAsync(string userId, string campaignTitle, bool accepted);
        Task NotifyNGOStatusAsync(string userId, bool approved, string? reason = null);
    }
}