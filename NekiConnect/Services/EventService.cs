using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;

namespace NekiConnect.Services
{
    public class EventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public EventService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Event>> GetByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Events.Where(e => e.NgoId == ngoId)
                                  .OrderBy(e => e.EventDate)
                                  .ToListAsync();
        }

        public async Task<List<Event>> GetUpcomingByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Events.Where(e => e.NgoId == ngoId
                                           && e.Status == "Upcoming"
                                           && e.EventDate >= DateTime.Today)
                                  .OrderBy(e => e.EventDate)
                                  .ToListAsync();
        }

        public async Task<List<Event>> GetAllUpcomingAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Events
                .Include(e => e.NGO)
                .Where(e => e.Status == "Upcoming" && e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            return await db.Events.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(Event ev)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Events.Add(ev);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event ev)
        {
            await using var db = await _factory.CreateDbContextAsync();
            db.Events.Update(ev);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();
            var ev = await db.Events.FindAsync(id);
            if (ev != null)
            {
                db.Events.Remove(ev);
                await db.SaveChangesAsync();
            }
        }
    }
}