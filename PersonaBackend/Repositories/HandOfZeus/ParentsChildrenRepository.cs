using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories.HandOfZeus
{
    public class ParentsChildrenRepository : IGenericRepository<ParentsChildren>, IDisposable
    {
        private Context _context;

        public ParentsChildrenRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<ParentsChildren> GetAll()
        {
            return _context.ParentsChildrens.ToList();
        }

        public Task<List<ParentsChildren>> GetAllAsync()
        {
            return _context.ParentsChildrens.ToListAsync();
        }

        public ParentsChildren GetById(int id)
        {
            return _context.ParentsChildrens.Find(id);
        }

        public async Task<ParentsChildren> GetByIdAsync(int id)
        {
            return await _context.ParentsChildrens.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var ParentsChildren = _context.ParentsChildrens.Find(id);
            if (ParentsChildren is { })
            {
                _context.ParentsChildrens.Remove(ParentsChildren);
                return true;
            }

            return false;
        }

        public void Add(in ParentsChildren sender)
        {
            _context.Add(sender).State = EntityState.Added;
        }

        public void Update(in ParentsChildren sender)
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

        public ParentsChildren GetByIdWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ParentsChildren> GetByIdWithIncludesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public ParentsChildren Select(Expression<Func<ParentsChildren, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<ParentsChildren> SelectAsync(Expression<Func<ParentsChildren, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
