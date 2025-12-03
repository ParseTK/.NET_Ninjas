using Microsoft.EntityFrameworkCore;
using SalesLedger.Models;

namespace SalesLedger.Data
{
    public class SalesLedgerDbContext : DbContext
    {
        public SalesLedgerDbContext(DbContextOptions<SalesLedgerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customers> Customers { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Products
            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(10, 2)");
            });
            // Orders
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(10, 2)");

                //relationship: Orders>Customers
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent accidental customer deletion if they have orders

                //relationship: Orders>Products
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Customers>(entity =>
            {
                // added a index on email for faster lookup
                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });
        }
    }
}
