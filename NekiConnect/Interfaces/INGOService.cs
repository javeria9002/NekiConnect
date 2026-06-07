using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface INGOService
    {
        Task RegisterNGOAsync(NGO ngo);
        Task<NGO?> GetNGOByUserIdAsync(string userId);
        Task<List<NGO>> GetAllApprovedNGOsAsync();
        Task<NGO?> GetNGOByIdAsync(int id);
        Task UpdateAsync(NGO ngo);
        Task<decimal> GetTotalDonationsAsync(int ngoId);
        Task<int> GetActiveCampaignsCountAsync(int ngoId);
        Task<int> GetUpcomingEventsCountAsync(int ngoId);
        Task<int> GetPendingVolunteersCountAsync(int ngoId);
        Task<Dictionary<int, double>> GetAverageRatingsAsync();
        Task<Dictionary<int, int>> GetRatingCountsAsync();
        Task<List<MonthlyDonation>> GetMonthlyDonationsAsync(int ngoId);
    }
}