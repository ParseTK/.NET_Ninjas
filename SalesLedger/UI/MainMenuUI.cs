using Spectre.Console;
using SalesLedger.Application.Interfaces;

namespace SalesLedger.UI
{
    /// Main menu UI handling
    public class MainMenuUI
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly CustomerMenuUI _customerMenu;
        private readonly ProductMenuUI _productMenu;
        private readonly OrderMenuUI _orderMenu;
        private readonly ReportsMenuUI _reportsMenu;

        public MainMenuUI(
            ICustomerService customerService,
            IProductService productService,
            IOrderService orderService,
            CustomerMenuUI customerMenu,
            ProductMenuUI productMenu,
            OrderMenuUI orderMenu,
            ReportsMenuUI reportsMenu)
        {
            _customerService = customerService;
            _productService = productService;
            _orderService = orderService;
            _customerMenu = customerMenu;
            _productMenu = productMenu;
            _orderMenu = orderMenu;
            _reportsMenu = reportsMenu;
        }

        public async Task RunAsync()
        {
            bool exitRequested = false;

            while (!exitRequested)
            {
                UIHelper.ClearScreen();
                await DisplayMainMenuAsync();

                var choice = UIHelper.PromptMenuChoice(
                    "Enter your choice [[1-4 or Q]]:",
                    c => c == '1' || c == '2' || c == '3' || c == '4' || 
                         char.ToUpper(c) == 'Q',
                    "Please enter 1-4 or Q"
                );

                switch (char.ToUpper(choice))
                {
                    case '1':
                        await _customerMenu.ShowAsync();
                        break;
                    case '2':
                        await _productMenu.ShowAsync();
                        break;
                    case '3':
                        await _orderMenu.ShowAsync();
                        break;
                    case '4':
                        await _reportsMenu.ShowAsync();
                        break;
                    case 'Q':
                        exitRequested = ConfirmExit();
                        break;
                }
            }

            ShowGoodbyeMessage();
        }

        private async Task DisplayMainMenuAsync()
        {
            // Breadcrumb
            UIHelper.ShowBreadcrumb("Main Menu");

            // Header
            UIHelper.ShowHeader("SALES LEDGER - MAIN MENU");

            // Menu options
            var menuTable = UIHelper.CreateMenuTable();
            menuTable.AddRow("[yellow]1.[/] Customer Management", "Manage customer records");
            menuTable.AddRow("[yellow]2.[/] Product Management", "Manage product catalog");
            menuTable.AddRow("[yellow]3.[/] Order Management", "Create and manage orders");
            menuTable.AddRow("[yellow]4.[/] Reports & Analytics", "View sales data and reports");
            menuTable.AddEmptyRow();
            menuTable.AddRow("[yellow]Q.[/] Exit Application", "Close the application");

            AnsiConsole.Write(menuTable);
            AnsiConsole.WriteLine();

            // System status panel
            await DisplaySystemStatusAsync();
        }

        private async Task DisplaySystemStatusAsync()
        {
            try
            {
                var customers = await _customerService.GetAllAsync();
                var products = await _productService.GetAllAsync();
                var orders = await _orderService.GetAllAsync();

                var statusContent = $"Database: [green]Connected[/]    " +
                                  $"Customers: {customers.Count}    " +
                                  $"Products: {products.Count}    " +
                                  $"Orders: {orders.Count}";

                var statusPanel = new Panel(statusContent)
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Grey),
                    Padding = new Padding(1, 0)
                }.Header("SYSTEM STATUS", Justify.Left);

                AnsiConsole.Write(statusPanel);
                AnsiConsole.WriteLine();
            }
            catch
            {
                var statusContent = "Database: [red]Disconnected[/]";
                var statusPanel = new Panel(statusContent)
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Red),
                    Padding = new Padding(1, 0)
                }.Header("SYSTEM STATUS", Justify.Left);

                AnsiConsole.Write(statusPanel);
                AnsiConsole.WriteLine();
                UIHelper.ShowWarning("Database connection failed. Please check your configuration.");
                AnsiConsole.WriteLine();
            }
        }

        private bool ConfirmExit()
        {
            return UIHelper.Confirm("Are you sure you want to exit?");
        }

        private void ShowGoodbyeMessage()
        {
            UIHelper.ClearScreen();
            string asciiArt = """
                   _____       _             _              _                 
                  ⧸ ____|     | |           | |            | |                
                 ⎹ (____  __ _| | ___  ___  | |     ___  __| | __ _  ___ _ __ 
                  \___  ⧹/ _` | |/ _ \/ __| | |    / _ \/ _` |/ _` |/ _ \ '__|
                  ____) | (_| | |  __/\__ \ | |___⎹  __/ (_| | (_| |  __/ |   
                 ⎹＿＿＿⧸\__,_|_|\___||___/ |______\___|\__,_|\__, |\___|_|   
                                                               __/ |          
                """;
            
            AnsiConsole.MarkupLine($"[blue]{asciiArt}[/]");
            AnsiConsole.MarkupLine($"[cyan] Thank you for using our SalesLedger![/]{"".PadRight(9)}[blue]⎹＿＿⧸[/]");
            AnsiConsole.MarkupLine("[grey] Developed by .NET_Ninjas[/]");
            AnsiConsole.MarkupLine($"[grey]{"Tyler".PadLeft(20)}[/]");
            AnsiConsole.MarkupLine($"[grey]{"Gabriel".PadLeft(22)}[/]");
            AnsiConsole.MarkupLine($"[grey]{"Alex".PadLeft(19)}[/]");
            AnsiConsole.MarkupLine($"[grey]{"Nick".PadLeft(19)}[/]\n");
        }
    }
}
