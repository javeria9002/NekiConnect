using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IEventService
    {
        Task<List<Event>> GetByNgoIdAsync(int ngoId);
        Task<List<Event>> GetUpcomingByNgoIdAsync(int ngoId);
        Task<List<Event>> GetAllUpcomingAsync();
        Task<Event?> GetByIdAsync(int id);
        Task CreateAsync(Event ev);
        Task UpdateAsync(Event ev);
        Task DeleteAsync(int id);
    }
}
