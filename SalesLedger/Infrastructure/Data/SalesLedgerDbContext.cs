using Microsoft.EntityFrameworkCore;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Data;
using SalesLedger.Infrastructure.Data.Configurations;
using SalesLedger.Infrastructure.Repositories;

namespace SalesLedger.Infrastructure.Data;

public class SalesLedgerDbContext : DbContext, IUnitOfWork
{
    public SalesLedgerDbContext(DbContextOptions<SalesLedgerDbContext> options)
        : base(options) { }

    public DbSet<Customers> Customers => Set<Customers>();
    public DbSet<Products> Products => Set<Products>();
    public DbSet<Orders> Orders => Set<Orders>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesLedgerDbContext).Assembly);

        modelBuilder.Seed();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await base.SaveChangesAsync(ct);
}