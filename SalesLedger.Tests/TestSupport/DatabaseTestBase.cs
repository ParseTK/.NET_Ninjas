using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SalesLedger.Infrastructure.Data;

namespace SalesLedger.Tests.TestSupport
{
    public abstract class DatabaseTestBase : IDisposable
    {
        protected readonly SalesLedgerDbContext Context;
        protected readonly string TestDatabaseName;
        private bool _disposed = false;

        protected DatabaseTestBase()
        {
    
            TestDatabaseName = TestDb.GenerateUniqueDatabaseName();

            var connectionString = TestDb.GetConnectionString(TestDatabaseName);

            var options = new DbContextOptionsBuilder<SalesLedgerDbContext>()
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging() 
                .Options;

            Context = new SalesLedgerDbContext(options);


            Context.Database.EnsureCreated();
        }

        protected Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Delete test database
                    Context.Database.EnsureDeleted();
                    Context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}