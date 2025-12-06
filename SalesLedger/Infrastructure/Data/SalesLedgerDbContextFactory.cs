using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SalesLedger.Infrastructure.Data;

/// Design-time factory for creating DbContext instances during migrations.
/// This allows 'dotnet ef' commands to work properly.
public class SalesLedgerDbContextFactory : IDesignTimeDbContextFactory<SalesLedgerDbContext>
{
    public SalesLedgerDbContext CreateDbContext(string[] args)
    {
        // Build configuration to read from user secrets
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<SalesLedgerDbContextFactory>()
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("SalesLedgerDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'SalesLedgerDb' not found. " +
                "Please configure it using: " +
                "dotnet user-secrets set \"ConnectionStrings:SalesLedgerDb\" \"your-connection-string\"");
        }

        // Create DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<SalesLedgerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SalesLedgerDbContext(optionsBuilder.Options);
    }
}
