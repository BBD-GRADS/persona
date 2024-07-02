using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories
{
    public class EventTypesRepository : IGenericRepository<EventType>, IDisposable
    {
        private readonly Context _context;

        public EventTypesRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<EventType> GetAll()
        {
            return _context.EventTypes.ToList();
        }

        public Task<List<EventType>> GetAllAsync()
        {
            return _context.EventTypes.ToListAsync();
        }

        public EventType GetById(int id)
        {
            return _context.EventTypes.Find(id);
        }

        public async Task<EventType> GetByIdAsync(int id)
        {
            return await _context.EventTypes.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var eventType = _context.EventTypes.Find(id);
            if (eventType is { })
            {
                _context.EventTypes.Remove(eventType);
                return true;
            }

            return false;
        }

        public void Add(in EventType eventType)
        {
            _context.EventTypes.Add(eventType);
        }

        public void Update(in EventType eventType)
        {
            _context.Entry(eventType).State = EntityState.Modified;
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
    }
}
