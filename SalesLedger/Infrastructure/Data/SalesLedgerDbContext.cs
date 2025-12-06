using Microsoft.EntityFrameworkCore;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Repositories;
using SalesLedger.Infrastructure.Data;

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
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await base.SaveChangesAsync(ct);
}