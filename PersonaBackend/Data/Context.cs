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
            modelBuilder.Entity<Persona>()
                .HasOne(p => p.NextOfKin)
                .WithMany()
                .HasForeignKey(p => p.NextOfKinId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.Partner)
                .WithMany()
                .HasForeignKey(p => p.PartnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.Parent)
                .WithMany()
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Persona>()
                .HasOne(p => p.HomeOwningStatus)
                .WithMany()
                .HasForeignKey(p => p.HomeOwningStatusId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StockItem>()
                .HasOne(si => si.Persona)
                .WithMany(p => p.StockInventory)
                .HasForeignKey(si => si.PersonaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StockItem>()
                .HasOne(si => si.Business)
                .WithMany()
                .HasForeignKey(si => si.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodItem>()
                .HasOne(fi => fi.Persona)
                .WithMany(p => p.FoodInventory)
                .HasForeignKey(fi => fi.PersonaId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}