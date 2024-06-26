using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories.HandOfZeus
{
    public class DiseasesRepository : IGenericRepository<Disease>, IDisposable
    {
        private Context _context;

        public DiseasesRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Disease> GetAll()
        {
            return _context.Diseases.ToList();
        }

        public Task<List<Disease>> GetAllAsync()
        {
            return _context.Diseases.ToListAsync();
        }

        public Disease GetById(int id)
        {
            return _context.Diseases.Find(id);
        }

        public async Task<Disease> GetByIdAsync(int id)
        {
            return await _context.Diseases.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var HomeOwningStatus = _context.HomeOwningStatuses.Find(id);
            if (HomeOwningStatus is { })
            {
                _context.HomeOwningStatuses.Remove(HomeOwningStatus);
                return true;
            }

            return false;
        }

        public void Add(in Disease sender)
        {
            _context.Add(sender).State = EntityState.Added;
        }

        public void Update(in Disease sender)
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

        public Disease GetByIdWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Disease> GetByIdWithIncludesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Disease Select(Expression<Func<Disease, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Disease> SelectAsync(Expression<Func<Disease, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
