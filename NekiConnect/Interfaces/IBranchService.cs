using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IBranchService
    {
        Task<List<NgoBranch>> GetByNgoIdAsync(int ngoId);
        Task CreateAsync(NgoBranch branch);
        Task DeleteAsync(int id);
    }
}