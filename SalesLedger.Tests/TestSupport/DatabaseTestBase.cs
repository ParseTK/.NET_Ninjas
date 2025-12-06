using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SalesLedger.Infrastructure.Data;

namespace SalesLedger.Tests.TestSupport
{
    /// Base class for all database tests. Provides common setup and teardown logic.
    public abstract class DatabaseTestBase : IDisposable
    {
        protected readonly SalesLedgerDbContext Context;
        protected readonly string TestDatabaseName;
        private bool _disposed = false;

        protected DatabaseTestBase()
        {
            // Generate unique test database name
            TestDatabaseName = TestDb.GenerateUniqueDatabaseName();

            // Get connection string and create context
            var connectionString = TestDb.GetConnectionString(TestDatabaseName);

            var options = new DbContextOptionsBuilder<SalesLedgerDbContext>()
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging() // Helpful for debugging tests
                .Options;

            Context = new SalesLedgerDbContext(options);

            // Create the database and apply schema
            Context.Database.EnsureCreated();
        }

        /// Creates a mock logger for a service
        protected Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        /// Cleans up test database and disposes resources
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