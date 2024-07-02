using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories
{
    public class HomeOwningStatusRepository : IGenericRepository<HomeOwningStatus>, IDisposable
    {
        private Context _context;

        public HomeOwningStatusRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<HomeOwningStatus> GetAll()
        {
            return _context.HomeOwningStatuses.ToList();
        }

        public Task<List<HomeOwningStatus>> GetAllAsync()
        {
            return _context.HomeOwningStatuses.ToListAsync();
        }

        public HomeOwningStatus GetById(int id)
        {
            return _context.HomeOwningStatuses.Find(id);
        }

        public async Task<HomeOwningStatus> GetByIdAsync(int id)
        {
            return await _context.HomeOwningStatuses.FindAsync(id);
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

        public void Add(in HomeOwningStatus sender)
        {
            _context.Add(sender).State = EntityState.Added;
        }

        public void Update(in HomeOwningStatus sender)
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
    }
}
