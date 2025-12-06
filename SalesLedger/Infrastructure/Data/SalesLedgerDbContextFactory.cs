using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SalesLedger.Infrastructure.Data
{
    public class SalesLedgerDbContextFactory : IDesignTimeDbContextFactory<SalesLedgerDbContext>
    {
        public SalesLedgerDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(typeof(SalesLedgerDbContextFactory).Assembly) // load secrets from this assembly
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SalesLedgerDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SalesLedgerDb"));

            return new SalesLedgerDbContext(optionsBuilder.Options);
        }
    }
}
