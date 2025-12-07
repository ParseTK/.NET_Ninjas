# Data
*This folder contains the Entity Framework Core DbContext and design-time factory for database operations and migrations.*

---

## 📁 Contents

- **SalesLedgerDbContext.cs:** *Main EF Core database context*
- **SalesLedgerDbContextFactory.cs:** *Design-time DbContext factory for migrations*

---

## 🗄️ SalesLedgerDbContext
*The main database context that manages entity tracking and database operations.*

- Provides DbSet<T> properties for querying entities
- Implements IUnitOfWork for transaction management
- Applies entity configurations from the Configurations folder
- Manages change tracking and database connections

**Implementation**
<details> <summary> click to expand </summary>

```csharp
public class SalesLedgerDbContext : DbContext, IUnitOfWork
{
    public SalesLedgerDbContext(DbContextOptions<SalesLedgerDbContext> options)
        : base(options) { }

  // DbSet properties for each entity
    public DbSet<Customers> Customers => Set<Customers>();
    public DbSet<Products> Products => Set<Products>();
    public DbSet<Orders> Orders => Set<Orders>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

  // Apply all entity configurations
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesLedgerDbContext).Assembly);
    }

   // Unit of Work implementation
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await base.SaveChangesAsync(ct);
}
```
</details>

---

## 🏭 SalesLedgerDbContextFactory
*Design-time factory that enables EF Core tooling to create a DbContext instance without running the application.*

- Enables dotnet ef migrations commands
- Loads connection string from User Secrets
- Creates DbContext for design-time operations

**Implementation**
<details> <summary> Click to Expand </summary>
  
```csharp
public class SalesLedgerDbContextFactory : IDesignTimeDbContextFactory<SalesLedgerDbContext>
{
    public SalesLedgerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(typeof(SalesLedgerDbContextFactory).Assembly)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SalesLedgerDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("SalesLedgerDb"));

        return new SalesLedgerDbContext(optionsBuilder.Options);
    }
}
```
</details>

---

## 🔧 Configuration
**Connection String Setup**

```powershell
# Set connection string in User Secrets
dotnet user-secrets set "ConnectionStrings:SalesLedgerDb" "Server=YOUR_SERVER;Database=SalesLedgerDb;Integrated Security=True;TrustServerCertificate=True"
```

---

## ✅ Best Practices

- **Single Responsibility:** *DbContext handles database operations only*
- **Explicit Transactions:** *Always call SaveChangesAsync() explicitly*
- **Async Operations:** *All database calls are asynchronous*
- **Configuration Separation:** *Entity configs in separate classes*
- **Factory Pattern:** *Design-time factory for tooling support*
- **Scoped Lifetime:** *DbContext registered as scoped service (per request)*

---

## 📊 Performance Considerations

- **Connection Pooling:** *Enabled by default in SQL Server provider*
- **Compiled Queries:** *Consider for frequently-executed queries*
- **Split Queries:** *Use .AsSplitQuery() for complex includes*
- **No Tracking:** *Use .AsNoTracking() for read-only queries*
