using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using SalesLedger.Infrastructure.Data;
using SalesLedger.Application.Interfaces;
using SalesLedger.Application.Services;
using SalesLedger.Infrastructure.Repositories;
using SalesLedger.UI;

namespace SalesLedger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddUserSecrets<Program>()
                    .Build();

                var connectionString = configuration.GetConnectionString("SalesLedgerDb");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    ShowConnectionStringError();
                    return;
                }

                var services = new ServiceCollection();
                ConfigureServices(services, connectionString);
                var serviceProvider = services.BuildServiceProvider();

                await TestDatabaseConnectionAsync(serviceProvider);

                var mainMenu = serviceProvider.GetRequiredService<MainMenuUI>();
                await mainMenu.RunAsync();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        private static void ConfigureServices(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SalesLedgerDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();

            services.AddScoped<CustomerMenuUI>();
            services.AddScoped<ProductMenuUI>();
            services.AddScoped<OrderMenuUI>();
            services.AddScoped<ReportsMenuUI>();
            services.AddScoped<MainMenuUI>();
        }

        private static async Task TestDatabaseConnectionAsync(ServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SalesLedgerDbContext>();

            try
            {
                await dbContext.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Failed to connect to database![/]");
                AnsiConsole.MarkupLine($"[grey]{ex.Message}[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Please ensure:[/]");
                AnsiConsole.MarkupLine("1. SQL Server is running");
                AnsiConsole.MarkupLine("2. Connection string is configured in user secrets");
                AnsiConsole.MarkupLine("3. Database exists (run migrations if needed)");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[grey]To set connection string, run:[/]");
                AnsiConsole.MarkupLine("[cyan]dotnet user-secrets set \"ConnectionStrings:SalesLedgerDb\" \"your-connection-string\"[/]");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static void ShowConnectionStringError()
        {
            UIHelper.ClearScreen();
            AnsiConsole.Write(
                new Panel(new Markup("[red]Connection string not found![/]"))
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Red),
                    Padding = new Padding(1, 0)
                }.Header("CONFIGURATION ERROR", Justify.Center));

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]The database connection string is not configured.[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]To configure your connection string:[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("1. Open a terminal in the project directory");
            AnsiConsole.MarkupLine("2. Run the following command:");
            AnsiConsole.WriteLine();
            
            var connectionStringExample = "Server=localhost;Database=SalesLedgerDb;Trusted_Connection=True;TrustServerCertificate=True";
            AnsiConsole.MarkupLine($"   [cyan]dotnet user-secrets set \"ConnectionStrings:SalesLedgerDb\" \"{connectionStringExample}\"[/]");
            
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Replace the connection string with your actual SQL Server details.[/]");
            AnsiConsole.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
