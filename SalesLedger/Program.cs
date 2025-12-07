using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using SalesLedger.Extensions;
using SalesLedger.Infrastructure.Data;
using SalesLedger.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

try
{
    var configuration = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

    var services = new ServiceCollection();
    services.AddSalesLedgerServices(configuration);

    // Register UI components
    services.AddScoped<CustomerMenuUI>();
    services.AddScoped<ProductMenuUI>();
    services.AddScoped<OrderMenuUI>();
    services.AddScoped<ReportsMenuUI>();
    services.AddScoped<MainMenuUI>();

    var provider = services.BuildServiceProvider();

    // Test database connection and apply migrations
    using (var scope = provider.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<SalesLedgerDbContext>();
        
        try
        {
            await db.Database.MigrateAsync();
            AnsiConsole.MarkupLine("[green]Database connected and migrations applied successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Database connection or migration failed![/]");
            AnsiConsole.WriteException(ex);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Press any key to continue anyway (UI will show disconnected)...[/]");
            Console.ReadKey();
        }
    }

    // Run the UI
    var mainMenu = provider.GetRequiredService<MainMenuUI>();
    await mainMenu.RunAsync();
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}