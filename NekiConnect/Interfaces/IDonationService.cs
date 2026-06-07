using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IDonationService
    {
        Task<List<Donation>> GetDonationsByDonorIdAsync(string donorId);
        Task<decimal> GetTotalDonatedAsync(string donorId);
        Task<decimal> GetThisMonthDonatedAsync(string donorId);
        Task<int> GetCampaignsSupportedCountAsync(string donorId);
        Task<Donation> AddDonationAsync(Donation donation);
    }
}