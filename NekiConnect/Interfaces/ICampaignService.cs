using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetAllActiveCampaignsAsync();
        Task<Campaign?> GetCampaignByIdAsync(int id);
        Task<List<Campaign>> GetCampaignsByNgoIdAsync(int ngoId);
        Task CreateCampaignAsync(Campaign campaign);
        Task UpdateCampaignAsync(Campaign campaign);
        Task DeleteCampaignAsync(int id);
        Task UpdateRaisedAmountAsync(int campaignId, decimal amount);
    }
}