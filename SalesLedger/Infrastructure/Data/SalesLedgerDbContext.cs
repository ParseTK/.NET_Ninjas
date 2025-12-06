using Microsoft.EntityFrameworkCore;
using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Data;

public class SalesLedgerDbContext : DbContext
{
    public SalesLedgerDbContext(DbContextOptions<SalesLedgerDbContext> options) : 
        base(options) { }

    public DbSet<Customers> Customers { get; set; }
    public DbSet<Products> Products { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Customers
        modelBuilder.Entity<Customers>(entity =>
        {
            entity.HasKey(c => c.CustomerId);
            entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(255);
        });

        // Configure Products
        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(p => p.ProductId);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });

        // Configure Orders
        modelBuilder.Entity<Orders>(entity =>
        {
            entity.HasKey(o => o.OrderId);
            entity.Property(o => o.OrderDate).IsRequired();

            // Relationship: Order belongs to Customer
            entity.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure OrderItem (composite key + relationships)
        modelBuilder.Entity<OrderItem>(entity =>
        {
            // Composite primary key
            entity.HasKey(oi => new { oi.OrderId, oi.ProductId });

            entity.Property(oi => oi.Quantity).IsRequired();
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");

            // Relationship: OrderItem belongs to Order
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: OrderItem references Product
            entity.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
