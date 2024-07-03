using Microsoft.EntityFrameworkCore;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona;

namespace PersonaBackend.Data
{
    public class Context : DbContext
    {
        public DbSet<Persona> Personas { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<HomeOwningStatus> HomeOwningStatuses { get; set; }
        public DbSet<EventOccurred> EventsOccurred { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        public Context()
        {
        }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }//TODO ADD homestatuses

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}