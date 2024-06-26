using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories.HandOfZeus
{
    public class StockIInventoryRepository : IGenericRepository<StockInventory>, IDisposable
    {
        private Context _context;

        public StockIInventoryRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<StockInventory> GetAll()
        {
            return _context.StockInventories.ToList();
        }

        public Task<List<StockInventory>> GetAllAsync()
        {
            return _context.StockInventories.ToListAsync();
        }

        public StockInventory GetById(int id)
        {
            return _context.StockInventories.Find(id);
        }

        public async Task<StockInventory> GetByIdAsync(int id)
        {
            return await _context.StockInventories.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var StockInventory = _context.StockInventories.Find(id);
            if (StockInventory is { })
            {
                _context.StockInventories.Remove(StockInventory);
                return true;
            }

            return false;
        }

        public void Add(in StockInventory sender)
        {
            _context.Add(sender).State = EntityState.Added;
        }

        public void Update(in StockInventory sender)
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

        public StockInventory GetByIdWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public Task<StockInventory> GetByIdWithIncludesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public StockInventory Select(Expression<Func<StockInventory, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<StockInventory> SelectAsync(Expression<Func<StockInventory, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
