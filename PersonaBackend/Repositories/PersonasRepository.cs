using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

#pragma warning disable CS8603

namespace PersonaBackend.Repositories
{
    public class PersonasRepository : IGenericRepository<Persona>, IDisposable
    {
        private Context _context;

        public PersonasRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Persona> GetAll()
        {
            return _context.Personas.ToList();
        }

        public Task<List<Persona>> GetAllAsync()
        {
            return _context.Personas.ToListAsync();
        }

        public Persona GetById(int id)
        {
            return _context.Personas.Find(id);
        }

        public async Task<Persona> GetByIdAsync(int id)
        {
            return await _context.Personas.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var Persona = _context.Personas.Find(id);
            if (Persona is { })
            {
                _context.Personas.Remove(Persona);
                return true;
            }

            return false;
        }

        public void Add(in Persona sender)
        {
            _context.Add(sender).State = EntityState.Added;
        }

        public void Update(in Persona sender)
        {
            _context.Entry(sender).State = EntityState.Modified;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Persona GetByIdWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Persona> GetByIdWithIncludesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Persona Select(Expression<Func<Persona, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Persona> SelectAsync(Expression<Func<Persona, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}