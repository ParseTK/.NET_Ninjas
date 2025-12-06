using Spectre.Console;
using SalesLedger.Application.Interfaces;
using SalesLedger.Domain;

namespace SalesLedger.UI
{
    /// Customer Management UI screens
    public class CustomerMenuUI
    {
        private readonly ICustomerService _customerService;

        public CustomerMenuUI(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task ShowAsync()
        {
            bool backToMain = false;

            while (!backToMain)
            {
                UIHelper.ClearScreen();
                DisplayCustomerMenu();

                var choice = UIHelper.PromptMenuChoice(
                    "Enter your choice [[1-6, 0, Q]]:",
                    c => (c >= '0' && c <= '6') || char.ToUpper(c) == 'Q',
                    "Please enter 0-6 or Q"
                );

                switch (choice)
                {
                    case '1':
                        await ViewAllCustomersAsync();
                        break;
                    case '2':
                        await SearchCustomerAsync();
                        break;
                    case '3':
                        await ViewCustomerOrderHistoryAsync();
                        break;
                    case '4':
                        await AddNewCustomerAsync();
                        break;
                    case '5':
                        await EditCustomerAsync();
                        break;
                    case '6':
                        await DeleteCustomerAsync();
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

        private void DisplayCustomerMenu()
        {
            UIHelper.ShowBreadcrumb("Customer Management");
            UIHelper.ShowHeader("CUSTOMER MANAGEMENT");

            // View & Search section
            var viewTable = UIHelper.CreateMenuTable("VIEW & SEARCH");
            viewTable.AddRow("[yellow]1.[/] View All Customers", "Display complete customer list");
            viewTable.AddRow("[yellow]2.[/] Search Customer", "Find by name, email, or ID");
            viewTable.AddRow("[yellow]3.[/] Customer Order History", "View a customer's orders");
            AnsiConsole.Write(viewTable);
            AnsiConsole.WriteLine();

            // Modify Records section
            var modifyTable = UIHelper.CreateMenuTable("MODIFY RECORDS");
            modifyTable.AddRow("[yellow]4.[/] Add New Customer", "Create new customer record");
            modifyTable.AddRow("[yellow]5.[/] Edit Customer", "Update existing customer info");
            modifyTable.AddRow("[yellow]6.[/] Delete Customer", "Remove customer (if no orders)");
            AnsiConsole.Write(modifyTable);
            AnsiConsole.WriteLine();

            // Navigation section
            var navTable = UIHelper.CreateMenuTable("NAVIGATION");
            navTable.AddRow("[yellow]0.[/] Back to Main Menu", "");
            navTable.AddRow("[yellow]Q.[/] Exit Application", "");
            AnsiConsole.Write(navTable);
            AnsiConsole.WriteLine();
        }

        private async Task ViewAllCustomersAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Customer Management > View All Customers");
            UIHelper.ShowHeader("CUSTOMER LIST");

            try
            {
                var customers = await UIHelper.WithSpinnerAsync(
                    "Loading customers...",
                    async () => await _customerService.GetAllAsync()
                );

                if (!customers.Any())
                {
                    UIHelper.ShowWarning("No customers found in the database.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Display count
                AnsiConsole.MarkupLine($"[grey]Total Customers: {customers.Count}[/]");
                AnsiConsole.WriteLine();

                // Create data table
                var table = UIHelper.CreateDataTable();
                table.AddColumn(new TableColumn("[bold]ID[/]").RightAligned());
                table.AddColumn("[bold]First Name[/]");
                table.AddColumn("[bold]Last Name[/]");
                table.AddColumn("[bold]Email[/]");
                table.AddColumn(new TableColumn("[bold]Orders[/]").RightAligned());

                foreach (var customer in customers.OrderBy(c => c.LastName))
                {
                    table.AddRow(
                        UIHelper.FormatGuid(customer.CustomerId),
                        Markup.Escape(customer.FirstName),
                        Markup.Escape(customer.LastName),
                        Markup.Escape(customer.Email),
                        customer.Orders.Count.ToString()
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();

                // Actions
                UIHelper.ShowInfo("Actions: [V] View Details | [E] Edit | [D] Delete | [0] Back");
                AnsiConsole.WriteLine();

                var action = UIHelper.PromptMenuChoice(
                    "Enter action (V/E/D) or 0 to go back:",
                    c => char.ToUpper(c) == 'V' || char.ToUpper(c) == 'E' || 
                         char.ToUpper(c) == 'D' || c == '0',
                    "Please enter V, E, D, or 0"
                );

                switch (char.ToUpper(action))
                {
                    case 'V':
                        await ViewCustomerDetailsAsync();
                        break;
                    case 'E':
                        await EditCustomerAsync();
                        break;
                    case 'D':
                        await DeleteCustomerAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error loading customers: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task ViewCustomerDetailsAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Customer Management > View Customer Details");

            var customerIdStr = UIHelper.PromptString("Enter Customer ID (first 8 characters) or 0 to cancel:");
            
            if (customerIdStr == "0") return;

            try
            {
                // Find customer by partial GUID match
                var allCustomers = await _customerService.GetAllAsync();
                var customer = allCustomers.FirstOrDefault(c => 
                    c.CustomerId.ToString().StartsWith(customerIdStr, StringComparison.OrdinalIgnoreCase));

                if (customer == null)
                {
                    UIHelper.ShowError("Customer not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Get full customer details with orders
                var customerWithOrders = await _customerService.GetWithOrdersAsync(customer.CustomerId);
                
                if (customerWithOrders == null)
                {
                    UIHelper.ShowError("Customer not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Customer Management > View Customer Details");
                UIHelper.ShowHeader($"CUSTOMER DETAILS - {customerWithOrders.FirstName} {customerWithOrders.LastName}");

                // Customer info panel
                var infoContent = $"[bold]Customer ID:[/] {UIHelper.FormatGuid(customerWithOrders.CustomerId)}\n" +
                                $"[bold]Name:[/] {Markup.Escape(customerWithOrders.FirstName)} {Markup.Escape(customerWithOrders.LastName)}\n" +
                                $"[bold]Email:[/] {Markup.Escape(customerWithOrders.Email)}\n" +
                                $"[bold]Total Orders:[/] {customerWithOrders.Orders.Count}";

                UIHelper.ShowPanel("CUSTOMER INFORMATION", infoContent, Color.Blue);

                if (customerWithOrders.Orders.Any())
                {
                    var ordersTable = UIHelper.CreateDataTable();
                    ordersTable.Title = new TableTitle("[bold]ORDER HISTORY[/]");
                    ordersTable.AddColumn("[bold]Order ID[/]");
                    ordersTable.AddColumn("[bold]Order Date[/]");
                    ordersTable.AddColumn(new TableColumn("[bold]Items[/]").RightAligned());

                    foreach (var order in customerWithOrders.Orders.OrderByDescending(o => o.OrderDate))
                    {
                        ordersTable.AddRow(
                            UIHelper.FormatGuid(order.OrderId),
                            order.OrderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                            order.Items.Count.ToString()
                        );
                    }

                    AnsiConsole.Write(ordersTable);
                    AnsiConsole.WriteLine();
                }

                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error retrieving customer details: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task SearchCustomerAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Customer Management > Search Customer");
            UIHelper.ShowHeader("SEARCH CUSTOMER");

            var searchOptions = new[]
            {
                "Search by First Name",
                "Search by Last Name",
                "Search by Email",
                "Cancel"
            };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select search method:")
                    .AddChoices(searchOptions)
            );

            if (choice == "Cancel") return;

            var searchTerm = UIHelper.PromptString("Enter search term:");
            
            if (string.IsNullOrWhiteSpace(searchTerm)) return;

            try
            {
                var allCustomers = await _customerService.GetAllAsync();
                IEnumerable<Customers> results;

                if (choice.Contains("First Name"))
                {
                    results = allCustomers.Where(c => 
                        c.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }
                else if (choice.Contains("Last Name"))
                {
                    results = allCustomers.Where(c => 
                        c.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }
                else // Email
                {
                    results = allCustomers.Where(c => 
                        c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }

                var resultsList = results.ToList();

                if (!resultsList.Any())
                {
                    UIHelper.ShowWarning("No customers found matching your search.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Customer Management > Search Results");
                UIHelper.ShowHeader($"SEARCH RESULTS ({resultsList.Count} found)");

                var table = UIHelper.CreateDataTable();
                table.AddColumn(new TableColumn("[bold]ID[/]").RightAligned());
                table.AddColumn("[bold]First Name[/]");
                table.AddColumn("[bold]Last Name[/]");
                table.AddColumn("[bold]Email[/]");

                foreach (var customer in resultsList)
                {
                    table.AddRow(
                        UIHelper.FormatGuid(customer.CustomerId),
                        Markup.Escape(customer.FirstName),
                        Markup.Escape(customer.LastName),
                        Markup.Escape(customer.Email)
                    );
                }

                AnsiConsole.Write(table);
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Search error: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task ViewCustomerOrderHistoryAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Customer Management > Customer Order History");

            var customerIdStr = UIHelper.PromptString("Enter Customer ID (first 8 characters) or 0 to cancel:");
            
            if (customerIdStr == "0") return;

            try
            {
                var allCustomers = await _customerService.GetAllAsync();
                var customer = allCustomers.FirstOrDefault(c => 
                    c.CustomerId.ToString().StartsWith(customerIdStr, StringComparison.OrdinalIgnoreCase));

                if (customer == null)
                {
                    UIHelper.ShowError("Customer not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                var customerWithOrders = await _customerService.GetWithOrdersAsync(customer.CustomerId);
                
                if (customerWithOrders == null || !customerWithOrders.Orders.Any())
                {
                    UIHelper.ShowWarning($"{customerWithOrders?.FirstName} {customerWithOrders?.LastName} has no orders.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Customer Management > Customer Order History");
                UIHelper.ShowHeader($"ORDER HISTORY - {customerWithOrders.FirstName} {customerWithOrders.LastName}");

                var table = UIHelper.CreateDataTable();
                table.AddColumn("[bold]Order ID[/]");
                table.AddColumn("[bold]Order Date[/]");
                table.AddColumn(new TableColumn("[bold]Total Items[/]").RightAligned());

                foreach (var order in customerWithOrders.Orders.OrderByDescending(o => o.OrderDate))
                {
                    table.AddRow(
                        UIHelper.FormatGuid(order.OrderId),
                        order.OrderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                        order.Items.Count.ToString()
                    );
                }

                AnsiConsole.Write(table);
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error loading order history: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task AddNewCustomerAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Customer Management > Add New Customer");
            UIHelper.ShowHeader("CREATE NEW CUSTOMER");

            // Show validation rules
            var rulesText = "✓ All fields are required\n" +
                          "✓ Email must be unique and contain '@'\n" +
                          "✓ Names should only contain letters and spaces";
            UIHelper.ShowPanel("VALIDATION RULES", rulesText, Color.Yellow);

            try
            {
                // Collect input
                var firstName = UIHelper.PromptRequiredString("First Name", 100);
                var lastName = UIHelper.PromptRequiredString("Last Name", 100);
                var email = UIHelper.PromptEmail();

                // Confirm
                AnsiConsole.WriteLine();
                var confirmContent = $"First Name: {Markup.Escape(firstName)}\n" +
                                   $"Last Name: {Markup.Escape(lastName)}\n" +
                                   $"Email: {Markup.Escape(email)}";
                UIHelper.ShowPanel("CONFIRM NEW CUSTOMER", confirmContent, Color.Green);

                if (!UIHelper.Confirm("Create this customer?"))
                {
                    UIHelper.ShowWarning("Customer creation cancelled.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Create customer
                var customer = new Customers
                {
                    CustomerId = Guid.NewGuid(),
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

                await UIHelper.WithSpinnerAsync(
                    "Creating customer...",
                    async () => await _customerService.CreateAsync(customer)
                );

                UIHelper.ShowSuccess($"Customer '{firstName} {lastName}' created successfully!");
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error creating customer: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task EditCustomerAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Customer Management > Edit Customer");

            var customerIdStr = UIHelper.PromptString("Enter Customer ID (first 8 characters) or 0 to cancel:");
            
            if (customerIdStr == "0") return;

            try
            {
                var allCustomers = await _customerService.GetAllAsync();
                var customer = allCustomers.FirstOrDefault(c => 
                    c.CustomerId.ToString().StartsWith(customerIdStr, StringComparison.OrdinalIgnoreCase));

                if (customer == null)
                {
                    UIHelper.ShowError("Customer not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Customer Management > Edit Customer");
                UIHelper.ShowHeader($"EDIT CUSTOMER - {customer.FirstName} {customer.LastName}");

                // Show current data
                var currentData = $"Current First Name: {Markup.Escape(customer.FirstName)}\n" +
                                $"Current Last Name: {Markup.Escape(customer.LastName)}\n" +
                                $"Current Email: {Markup.Escape(customer.Email)}";
                UIHelper.ShowPanel("CURRENT INFORMATION", currentData, Color.Grey);

                // Get new values (allow keeping current via default)
                AnsiConsole.MarkupLine("[grey]Press Enter to keep current value[/]");
                AnsiConsole.WriteLine();

                var firstName = AnsiConsole.Prompt(
                    new TextPrompt<string>("First Name:")
                        .DefaultValue(customer.FirstName)
                        .AllowEmpty()
                ) ?? customer.FirstName;

                var lastName = AnsiConsole.Prompt(
                    new TextPrompt<string>("Last Name:")
                        .DefaultValue(customer.LastName)
                        .AllowEmpty()
                ) ?? customer.LastName;

                var email = AnsiConsole.Prompt(
                    new TextPrompt<string>("Email:")
                        .DefaultValue(customer.Email)
                        .AllowEmpty()
                        .Validate(e =>
                        {
                            if (!string.IsNullOrWhiteSpace(e) && !e.Contains('@'))
                                return ValidationResult.Error("Email must contain '@'");
                            return ValidationResult.Success();
                        })
                ) ?? customer.Email;

                // Confirm changes
                if (!UIHelper.Confirm("Save changes?"))
                {
                    UIHelper.ShowWarning("Edit cancelled.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Update
                customer.FirstName = firstName;
                customer.LastName = lastName;
                customer.Email = email;

                await UIHelper.WithSpinnerAsync(
                    "Updating customer...",
                    async () => await _customerService.UpdateAsync(customer)
                );

                UIHelper.ShowSuccess("Customer updated successfully!");
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error updating customer: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task DeleteCustomerAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Customer Management > Delete Customer");
            UIHelper.ShowHeader("DELETE CUSTOMER");

            var customerIdStr = UIHelper.PromptString("Enter Customer ID (first 8 characters) or 0 to cancel:");
            
            if (customerIdStr == "0") return;

            try
            {
                var allCustomers = await _customerService.GetAllAsync();
                var customer = allCustomers.FirstOrDefault(c => 
                    c.CustomerId.ToString().StartsWith(customerIdStr, StringComparison.OrdinalIgnoreCase));

                if (customer == null)
                {
                    UIHelper.ShowError("Customer not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Get customer with orders to check
                var customerWithOrders = await _customerService.GetWithOrdersAsync(customer.CustomerId);

                if (customerWithOrders == null)
                {
                    UIHelper.ShowError("Customer not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Check for orders
                if (customerWithOrders.Orders.Any())
                {
                    UIHelper.ShowError($"Cannot delete customer with existing orders (Order count: {customerWithOrders.Orders.Count})");
                    UIHelper.ShowInfo("You must delete all orders first or mark the customer as inactive.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Show customer info and confirm deletion
                var customerInfo = $"{customerWithOrders.FirstName} {customerWithOrders.LastName} ({customerWithOrders.Email})";
                
                if (!UIHelper.ConfirmDeletion(customerInfo))
                {
                    UIHelper.ShowWarning("Deletion cancelled.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Delete
                await UIHelper.WithSpinnerAsync(
                    "Deleting customer...",
                    async () => await _customerService.DeleteAsync(customerWithOrders.CustomerId)
                );

                UIHelper.ShowSuccess("Customer deleted successfully!");
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error deleting customer: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }
    }
}
