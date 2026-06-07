using Microsoft.EntityFrameworkCore;
using NekiConnect.Data;
using NekiConnect.Models;
using NekiConnect.Interfaces;

namespace NekiConnect.Services
{
    public class EventService : IEventService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public EventService(IDbContextFactory<ApplicationDbContext> factory)
        {
            _factory = factory;
        }

        // ── NGO events ──
        public async Task<List<Event>> GetByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Events
                .Where(e => e.NgoId == ngoId)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        // ── Upcoming NGO events ──
        public async Task<List<Event>> GetUpcomingByNgoIdAsync(int ngoId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Events
                .Where(e =>
                    e.NgoId == ngoId &&
                    e.Status == "Upcoming" &&
                    e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        // ── Global upcoming events ──
        public async Task<List<Event>> GetAllUpcomingAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Events
                .Include(e => e.NGO)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        // ── Single event ──
        public async Task<Event?> GetByIdAsync(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Events
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // ── Create ──
        public async Task CreateAsync(Event ev)
        {
            await using var db = await _factory.CreateDbContextAsync();

            db.Events.Add(ev);
            await db.SaveChangesAsync();
        }

        // ── Update ──
        public async Task UpdateAsync(Event ev)
        {
            await using var db = await _factory.CreateDbContextAsync();

            db.Events.Update(ev);
            await db.SaveChangesAsync();
        }

        // ── Delete ──
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