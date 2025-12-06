using Spectre.Console;
using SalesLedger.Application.Interfaces;

namespace SalesLedger.UI
{
    /// Reports & Analytics UI screens
    public class ReportsMenuUI
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public ReportsMenuUI(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
        }

        public async Task ShowAsync()
        {
            bool backToMain = false;

            while (!backToMain)
            {
                UIHelper.ClearScreen();
                DisplayReportsMenu();

                var choice = UIHelper.PromptMenuChoice(
                    "Enter your choice [[1-3, 0, Q]]:",
                    c => (c >= '0' && c <= '3') || char.ToUpper(c) == 'Q',
                    "Please enter 0-3 or Q"
                );

                switch (choice)
                {
                    case '1':
                        await ShowSalesSummaryAsync();
                        break;
                    case '2':
                        await ShowTopCustomersAsync();
                        break;
                    case '3':
                        await ShowTopProductsAsync();
                        break;
                    case '0':
                        backToMain = true;
                        break;
                    case 'Q':
                    case 'q':
                        if (UIHelper.Confirm("Are you sure you want to exit?"))
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
            }
        }

        private void DisplayReportsMenu()
        {
            UIHelper.ShowBreadcrumb("Reports & Analytics");
            UIHelper.ShowHeader("REPORTS & ANALYTICS");

            var reportsTable = UIHelper.CreateMenuTable("AVAILABLE REPORTS");
            reportsTable.AddRow("[yellow]1.[/] Sales Summary", "Overall sales statistics");
            reportsTable.AddRow("[yellow]2.[/] Top Customers", "Customers by order count");
            reportsTable.AddRow("[yellow]3.[/] Top Products", "Most ordered products");
            AnsiConsole.Write(reportsTable);
            AnsiConsole.WriteLine();

            var navTable = UIHelper.CreateMenuTable("NAVIGATION");
            navTable.AddRow("[yellow]0.[/] Back to Main Menu", "");
            navTable.AddRow("[yellow]Q.[/] Exit Application", "");
            AnsiConsole.Write(navTable);
            AnsiConsole.WriteLine();
        }

        private async Task ShowSalesSummaryAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Reports & Analytics > Sales Summary");
            UIHelper.ShowHeader("SALES SUMMARY REPORT");

            try
            {
                var orders = await UIHelper.WithSpinnerAsync(
                    "Generating report...",
                    async () => await _orderService.GetAllAsync()
                );

                var customers = await _customerService.GetAllAsync();
                var products = await _productService.GetAllAsync();

                // Calculate totals
                decimal totalRevenue = 0;
                int totalItemsSold = 0;

                foreach (var order in orders)
                {
                    foreach (var item in order.Items)
                    {
                        totalRevenue += item.Quantity * item.UnitPrice;
                        totalItemsSold += item.Quantity;
                    }
                }

                var avgOrderValue = orders.Any() ? totalRevenue / orders.Count : 0;

                // Display summary
                var summaryContent = $"[bold]Total Orders:[/] {orders.Count}\n" +
                                   $"[bold]Total Revenue:[/] {totalRevenue:C2}\n" +
                                   $"[bold]Average Order Value:[/] {avgOrderValue:C2}\n" +
                                   $"[bold]Total Items Sold:[/] {totalItemsSold}\n" +
                                   $"[bold]Total Customers:[/] {customers.Count}\n" +
                                   $"[bold]Total Products:[/] {products.Count}";

                UIHelper.ShowPanel("OVERALL STATISTICS", summaryContent, Color.Green);

                // Recent orders
                if (orders.Any())
                {
                    var recentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5);
                    var table = UIHelper.CreateDataTable();
                    table.Title = new TableTitle("[bold]RECENT ORDERS (Last 5)[/]");
                    table.AddColumn("[bold]Order ID[/]");
                    table.AddColumn("[bold]Customer[/]");
                    table.AddColumn("[bold]Date[/]");
                    table.AddColumn(new TableColumn("[bold]Total[/]").RightAligned());

                    foreach (var order in recentOrders)
                    {
                        var customerName = order.Customer != null 
                            ? $"{order.Customer.FirstName} {order.Customer.LastName}"
                            : "Unknown";
                        var total = order.Items.Sum(i => i.Quantity * i.UnitPrice);

                        table.AddRow(
                            UIHelper.FormatGuid(order.OrderId),
                            Markup.Escape(customerName),
                            order.OrderDate.ToLocalTime().ToString("yyyy-MM-dd"),
                            total.ToString("C2")
                        );
                    }

                    AnsiConsole.Write(table);
                    AnsiConsole.WriteLine();
                }

                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error generating report: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task ShowTopCustomersAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Reports & Analytics > Top Customers");
            UIHelper.ShowHeader("TOP CUSTOMERS REPORT");

            try
            {
                var customers = await UIHelper.WithSpinnerAsync(
                    "Generating report...",
                    async () => await _customerService.GetAllAsync()
                );

                // Calculate customer statistics
                var customerStats = customers.Select(c => new
                {
                    Customer = c,
                    OrderCount = c.Orders.Count,
                    TotalRevenue = c.Orders.SelectMany(o => o.Items)
                        .Sum(i => i.Quantity * i.UnitPrice)
                })
                .OrderByDescending(cs => cs.OrderCount)
                .Take(10)
                .ToList();

                if (!customerStats.Any())
                {
                    UIHelper.ShowWarning("No customer data available.");
                    UIHelper.PressAnyKey();
                    return;
                }

                var table = UIHelper.CreateDataTable();
                table.Title = new TableTitle("[bold]TOP 10 CUSTOMERS BY ORDER COUNT[/]");
                table.AddColumn(new TableColumn("[bold]Rank[/]").RightAligned());
                table.AddColumn("[bold]Customer Name[/]");
                table.AddColumn("[bold]Email[/]");
                table.AddColumn(new TableColumn("[bold]Orders[/]").RightAligned());
                table.AddColumn(new TableColumn("[bold]Total Spent[/]").RightAligned());

                int rank = 1;
                foreach (var stat in customerStats)
                {
                    table.AddRow(
                        rank.ToString(),
                        $"{Markup.Escape(stat.Customer.FirstName)} {Markup.Escape(stat.Customer.LastName)}",
                        Markup.Escape(stat.Customer.Email),
                        stat.OrderCount.ToString(),
                        stat.TotalRevenue.ToString("C2")
                    );
                    rank++;
                }

                AnsiConsole.Write(table);
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error generating report: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task ShowTopProductsAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Reports & Analytics > Top Products");
            UIHelper.ShowHeader("TOP PRODUCTS REPORT");

            try
            {
                var products = await UIHelper.WithSpinnerAsync(
                    "Generating report...",
                    async () => await _productService.GetAllAsync()
                );

                // Calculate product statistics
                var productStats = products.Select(p => new
                {
                    Product = p,
                    TimesSold = p.OrderItems.Count,
                    QuantitySold = p.OrderItems.Sum(oi => oi.Quantity),
                    TotalRevenue = p.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(ps => ps.QuantitySold)
                .Take(10)
                .ToList();

                if (!productStats.Any())
                {
                    UIHelper.ShowWarning("No product sales data available.");
                    UIHelper.PressAnyKey();
                    return;
                }

                var table = UIHelper.CreateDataTable();
                table.Title = new TableTitle("[bold]TOP 10 PRODUCTS BY QUANTITY SOLD[/]");
                table.AddColumn(new TableColumn("[bold]Rank[/]").RightAligned());
                table.AddColumn("[bold]Product Name[/]");
                table.AddColumn(new TableColumn("[bold]Price[/]").RightAligned());
                table.AddColumn(new TableColumn("[bold]Qty Sold[/]").RightAligned());
                table.AddColumn(new TableColumn("[bold]Revenue[/]").RightAligned());

                int rank = 1;
                foreach (var stat in productStats)
                {
                    table.AddRow(
                        rank.ToString(),
                        Markup.Escape(stat.Product.Name),
                        stat.Product.Price.ToString("C2"),
                        stat.QuantitySold.ToString(),
                        stat.TotalRevenue.ToString("C2")
                    );
                    rank++;
                }

                AnsiConsole.Write(table);
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error generating report: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }
    }
}
