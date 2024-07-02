using Microsoft.EntityFrameworkCore;
using PersonaBackend.Data;
using PersonaBackend.Interfaces;
using PersonaBackend.Models.Persona;
using System.Linq.Expressions;

namespace PersonaBackend.Repositories
{
    public class FoodInventoryRepository : IGenericRepository<FoodInventory>, IDisposable
    {
        private readonly Context _context;

        public FoodInventoryRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<FoodInventory> GetAll()
        {
            return _context.FoodInventories.ToList();
        }

        public Task<List<FoodInventory>> GetAllAsync()
        {
            return _context.FoodInventories.ToListAsync();
        }

        public FoodInventory GetById(int id)
        {
            return _context.FoodInventories.Find(id);
        }

        public async Task<FoodInventory> GetByIdAsync(int id)
        {
            return await _context.FoodInventories.FindAsync(id);
        }

        public bool Remove(int id)
        {
            var food = _context.FoodInventories.Find(id);
            if (food is { })
            {
                _context.FoodInventories.Remove(food);
                return true;
            }

            return false;
        }

        public void Add(in FoodInventory food)
        {
            _context.FoodInventories.Add(food);
        }

        public void Update(in FoodInventory food)
        {
            _context.Entry(food).State = EntityState.Modified;
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
