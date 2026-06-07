using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task UpdateUserAsync(ApplicationUser updated);
        Task SuspendUserAsync(string id);
        Task UnsuspendUserAsync(string id);
        Task DeleteUserAsync(string id);
    }
}