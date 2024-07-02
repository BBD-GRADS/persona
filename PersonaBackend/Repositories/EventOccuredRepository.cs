using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories
{
    public class EventOccurredRepository : IGenericRepository<EventOccurred>, IDisposable
    {
        private readonly Context _context;

        public EventOccurredRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<EventOccurred> GetAll()
        {
            return _context.EventsOccurred.Include(e => e.EventType).ToList();
        }

        public Task<List<EventOccurred>> GetAllAsync()
        {
            return _context.EventsOccurred.Include(e => e.EventType).ToListAsync();
        }

        public EventOccurred GetById(int id)
        {
            return _context.EventsOccurred.Find(id);
        }

        public async Task<EventOccurred> GetByIdAsync(int id)
        {
            return await _context.EventsOccurred.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var @event = _context.EventsOccurred.Find(id);
            if (@event is { })
            {
                _context.EventsOccurred.Remove(@event);
                return true;
            }

            return false;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Add(in EventOccurred sender)
        {
            _context.EventsOccurred.Add(sender);
        }

        public void Update(in EventOccurred sender)
        {
            _context.Entry(sender).State = EntityState.Modified;
        }
    }
}
