using Microsoft.EntityFrameworkCore;
using PersonaBackend.Models.Persona;

namespace PersonaBackend.Data
{
    public class Context : DbContext
    {
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<HomeOwningStatus> HomeOwningStatuses { get; set; }
        public DbSet<StockInventory> StockInventories { get; set; }
        public DbSet<ParentsChildren> ParentsChildrens { get; set; }

        public Context()
        {
        }
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
    }

}

