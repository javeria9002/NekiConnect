using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IFeedbackService
    {
        // Fetch
        Task<List<Feedback>> GetByNgoIdAsync(int ngoId);
        Task<bool> HasReviewedAsync(string userId, int ngoId);

        // Verified check — has this user donated or volunteered with this NGO?
        Task<bool> IsVerifiedDonorAsync(string userId, int ngoId);
        Task<bool> IsVerifiedVolunteerAsync(string userId, int ngoId);

        // Returns "Donor", "Volunteer", or null (not eligible)
        Task<string?> GetVerifiedTypeAsync(string userId, int ngoId);

        // Write
        Task AddAsync(Feedback feedback);
        Task DeleteAsync(int id);
    }
}