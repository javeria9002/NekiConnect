using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IBeneficiaryService
    {
        Task<List<Beneficiary>> GetByNgoIdAsync(int ngoId);
        Task<Beneficiary?> GetByIdAsync(int id);
        Task CreateAsync(Beneficiary b);
        Task UpdateAsync(Beneficiary b);
        Task DeleteAsync(int id);
    }
}