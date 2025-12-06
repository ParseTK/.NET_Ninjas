using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesLedger.Extensions;
using SalesLedger.Infrastructure.Data;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>() 
    .Build();

var services = new ServiceCollection();
services.AddSalesLedgerServices(configuration);

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<SalesLedgerDbContext>();
await db.Database.MigrateAsync();

Console.WriteLine("SalesLedger started — connected via User Secrets only.");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();