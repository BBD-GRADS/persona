using Microsoft.EntityFrameworkCore;
using PersonaBackend.Models.HandOfZeus;
using PersonaBackend.Models.Persona;

namespace PersonaBackend.Data
{
    public class Context : DbContext
    {
        public DbSet<Persona> Personas { get; set; }
        public DbSet<StockInventory> StockInventories { get; set; }
        public DbSet<FoodInventory> FoodInventories { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<HomeOwningStatus> HomeOwningStatuses { get; set; }
        public DbSet<EventOccurred> EventsOccurred { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        public Context()
        {
        }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persona>()
                .HasOne(p => p.NextOfKin)
                .WithMany()
                .HasForeignKey(p => p.NextOfKinId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.Partner)
                .WithMany()
                .HasForeignKey(p => p.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.Parent)
                .WithMany()
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.HomeOwningStatus)
                .WithMany()
                .HasForeignKey(p => p.HomeOwningStatusId);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.FoodInventory)
                .WithMany()
                .HasForeignKey(p => p.FoodInventoryId);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.StockInventory)
                .WithMany()
                .HasForeignKey(p => p.StockInventoryId);

            modelBuilder.Entity<StockInventory>()
                .HasOne(s => s.Business)
                .WithMany()
                .HasForeignKey(s => s.BusinessId);

            modelBuilder.Entity<EventOccurred>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventId);

            modelBuilder.Entity<EventOccurred>()
                .HasOne(e => e.Persona1)
                .WithMany()
                .HasForeignKey(e => e.PersonaId1);

            modelBuilder.Entity<EventOccurred>()
                .HasOne(e => e.Persona2)
                .WithMany()
                .HasForeignKey(e => e.PersonaId2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
