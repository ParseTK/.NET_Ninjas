using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace SalesLedger.Tests.TestSupport
{
    public static class TestDb
    {
        private const string TEST_DATABASE_BASE_NAME = "SalesLedgerDbTest";

        public static string GenerateUniqueDatabaseName()
        {
            return $"{TEST_DATABASE_BASE_NAME}_{Guid.NewGuid():N}";
        }

        public static string GetConnectionString(string databaseName)
        {
            var baseConnectionString = LoadBaseConnectionString();

            return baseConnectionString.Replace(TEST_DATABASE_BASE_NAME, databaseName);
        }

        public static string GetBaseConnectionString()
        {
            return LoadBaseConnectionString();
        }

        private static string LoadBaseConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<DatabaseTestBase>()
                .Build();

            var connectionString = configuration.GetConnectionString(TEST_DATABASE_BASE_NAME);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{TEST_DATABASE_BASE_NAME}' not found in user secrets. " +
                    $"Run: dotnet user-secrets set \"ConnectionStrings:{TEST_DATABASE_BASE_NAME}\" \"your-connection-string\"");
            }

            return connectionString;
        }
    }
}
