using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories.HandOfZeus
{
    public class BusinessesRepository : IGenericRepository<Business>, IDisposable
    {
        private Context _context;

        public BusinessesRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Business> GetAll()
        {
            return _context.Businesses.ToList();
        }

        public Task<List<Business>> GetAllAsync()
        {
            return _context.Businesses.ToListAsync();
        }

        public Business GetById(int id)
        {
            return _context.Businesses.Find(id);
        }

        public async Task<Business> GetByIdAsync(int id)
        {
            return await _context.Businesses.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var Business = _context.Businesses.Find(id);
            if (Business is { })
            {
                _context.Businesses.Remove(Business);
                return true;
            }

            return false;
        }

        public void Add(in Business sender)
        {
            _context.Add(sender).State = EntityState.Added;
        }

        public void Update(in Business sender)
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

        public Business GetByIdWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Business> GetByIdWithIncludesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Business Select(Expression<Func<Business, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Business> SelectAsync(Expression<Func<Business, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
