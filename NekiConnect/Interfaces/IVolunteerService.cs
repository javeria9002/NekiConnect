using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IVolunteerService
    {
        Task<List<VolunteerApplication>> GetApplicationsByNgoIdAsync(int ngoId);
        Task<List<VolunteerApplication>> GetApplicationsByUserIdAsync(string userId);
        Task<List<VolunteerApplication>> GetApplicationsByCampaignIdAsync(int campaignId);
        Task<List<VolunteerApplication>> GetApplicationsByEventIdAsync(int eventId);
        Task<bool> HasAppliedAsync(string userId, int campaignId);
        Task<bool> HasAppliedToEventAsync(string userId, int eventId);
        Task ApplyAsync(VolunteerApplication application);
        Task UpdateStatusAsync(int applicationId, string status);
        Task<int> GetApplicationsCountAsync(string userId);
    }
}