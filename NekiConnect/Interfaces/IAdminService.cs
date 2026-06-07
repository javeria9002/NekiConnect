using NekiConnect.Models;


namespace NekiConnect.Interfaces
{
    public interface IAdminService
    {
        // NGO Approvals
        Task<List<NGO>> GetAllNGOsAsync();
        Task<List<NGO>> GetNGOsByStatusAsync(string status);
        Task ApproveNGOAsync(int ngoId);
        Task RejectNGOAsync(int ngoId, string reason);
        Task SuspendNGOAsync(int ngoId);
        Task DeleteNGOAsync(int ngoId);

        // Stats
        Task<int> GetTotalNGOsAsync();
        Task<int> GetPendingNGOsCountAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetActiveCampaignsCountAsync();
        Task<decimal> GetTotalDonationsAsync();

        // Charts & Overviews
        Task<List<MonthlyDonation>> GetPlatformMonthlyAsync();
        Task<List<CategoryTotal>> GetDonationsByCategoryAsync();
        Task<List<UserOverview>> GetUserOverviewAsync();
        Task<List<Donation>> GetAllDonationsAsync();
        Task<List<NgoOverview>> GetNgoOverviewAsync();
    }
}