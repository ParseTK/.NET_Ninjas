using Spectre.Console;
using SalesLedger.Application.Interfaces;
using SalesLedger.Domain;

namespace SalesLedger.UI
{
    /// Order Management UI screens
    public class OrderMenuUI
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderItemService _orderItemService;

        public OrderMenuUI(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            IOrderItemService orderItemService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _orderItemService = orderItemService;
        }

        public async Task ShowAsync()
        {
            bool backToMain = false;

            while (!backToMain)
            {
                UIHelper.ClearScreen();
                DisplayOrderMenu();

                var choice = UIHelper.PromptMenuChoice(
                    "Enter your choice [[1-4, 0, Q]]:",
                    c => (c >= '0' && c <= '4') || char.ToUpper(c) == 'Q',
                    "Please enter 0-4 or Q"
                );

                switch (choice)
                {
                    case '1':
                        await ViewAllOrdersAsync();
                        break;
                    case '2':
                        await CreateNewOrderWizardAsync();
                        break;
                    case '3':
                        await ViewOrderDetailsAsync();
                        break;
                    case '4':
                        await DeleteOrderAsync();
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

        private void DisplayOrderMenu()
        {
            UIHelper.ShowBreadcrumb("Order Management");
            UIHelper.ShowHeader("ORDER MANAGEMENT");

            // View section
            var viewTable = UIHelper.CreateMenuTable("VIEW & SEARCH");
            viewTable.AddRow("[yellow]1.[/] View All Orders", "Display all orders");
            viewTable.AddRow("[yellow]3.[/] View Order Details", "View specific order information");
            AnsiConsole.Write(viewTable);
            AnsiConsole.WriteLine();

            // Create section
            var createTable = UIHelper.CreateMenuTable("CREATE");
            createTable.AddRow("[yellow]2.[/] Create New Order", "Multi-step order creation wizard");
            AnsiConsole.Write(createTable);
            AnsiConsole.WriteLine();

            // Modify section
            var modifyTable = UIHelper.CreateMenuTable("MODIFY RECORDS");
            modifyTable.AddRow("[yellow]4.[/] Delete Order", "Remove an order");
            AnsiConsole.Write(modifyTable);
            AnsiConsole.WriteLine();

            // Navigation section
            var navTable = UIHelper.CreateMenuTable("NAVIGATION");
            navTable.AddRow("[yellow]0.[/] Back to Main Menu", "");
            navTable.AddRow("[yellow]Q.[/] Exit Application", "");
            AnsiConsole.Write(navTable);
            AnsiConsole.WriteLine();
        }

        private async Task ViewAllOrdersAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Order Management > View All Orders");
            UIHelper.ShowHeader("ORDER LIST");

            try
            {
                var orders = await UIHelper.WithSpinnerAsync(
                    "Loading orders...",
                    async () => await _orderService.GetAllAsync()
                );

                if (!orders.Any())
                {
                    UIHelper.ShowWarning("No orders found in the database.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Display count
                AnsiConsole.MarkupLine($"[grey]Total Orders: {orders.Count}[/]");
                AnsiConsole.WriteLine();

                // Create data table
                var table = UIHelper.CreateDataTable();
                table.AddColumn(new TableColumn("[bold]Order ID[/]").RightAligned());
                table.AddColumn("[bold]Customer[/]");
                table.AddColumn("[bold]Order Date[/]");
                table.AddColumn(new TableColumn("[bold]Items[/]").RightAligned());
                table.AddColumn(new TableColumn("[bold]Total[/]").RightAligned());

                foreach (var order in orders.OrderByDescending(o => o.OrderDate))
                {
                    var customerName = order.Customer != null 
                        ? $"{order.Customer.FirstName} {order.Customer.LastName}"
                        : "Unknown";
                    
                    var total = order.Items.Sum(i => i.Quantity * i.UnitPrice);

                    table.AddRow(
                        UIHelper.FormatGuid(order.OrderId),
                        Markup.Escape(customerName),
                        order.OrderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                        order.Items.Count.ToString(),
                        total.ToString("C2")
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();

                // Actions
                UIHelper.ShowInfo("Actions: [V] View Details | [D] Delete | [0] Back");
                AnsiConsole.WriteLine();

                var action = UIHelper.PromptMenuChoice(
                    "Enter action (V/D) or 0 to go back:",
                    c => char.ToUpper(c) == 'V' || char.ToUpper(c) == 'D' || c == '0',
                    "Please enter V, D, or 0"
                );

                switch (char.ToUpper(action))
                {
                    case 'V':
                        await ViewOrderDetailsAsync();
                        break;
                    case 'D':
                        await DeleteOrderAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error loading orders: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task ViewOrderDetailsAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Order Management > View Order Details");

            var orderIdStr = UIHelper.PromptString("Enter Order ID (first 8 characters) or 0 to cancel:");
            
            if (orderIdStr == "0") return;

            try
            {
                var allOrders = await _orderService.GetAllAsync();
                var order = allOrders.FirstOrDefault(o => 
                    o.OrderId.ToString().StartsWith(orderIdStr, StringComparison.OrdinalIgnoreCase));

                if (order == null)
                {
                    UIHelper.ShowError("Order not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Order Management > View Order Details");
                UIHelper.ShowHeader($"ORDER DETAILS - {UIHelper.FormatGuid(order.OrderId)}");

                // Order info panel
                var customerName = order.Customer != null 
                    ? $"{order.Customer.FirstName} {order.Customer.LastName}"
                    : "Unknown";

                var infoContent = $"[bold]Order ID:[/] {UIHelper.FormatGuid(order.OrderId)}\n" +
                                $"[bold]Customer:[/] {Markup.Escape(customerName)}\n" +
                                $"[bold]Order Date:[/] {order.OrderDate.ToLocalTime():yyyy-MM-dd HH:mm}\n" +
                                $"[bold]Total Items:[/] {order.Items.Count}";

                UIHelper.ShowPanel("ORDER INFORMATION", infoContent, Color.Blue);

                // Items table
                if (order.Items.Any())
                {
                    var itemsTable = UIHelper.CreateDataTable();
                    itemsTable.Title = new TableTitle("[bold]ORDER ITEMS[/]");
                    itemsTable.AddColumn("[bold]Product[/]");
                    itemsTable.AddColumn(new TableColumn("[bold]Qty[/]").RightAligned());
                    itemsTable.AddColumn(new TableColumn("[bold]Unit Price[/]").RightAligned());
                    itemsTable.AddColumn(new TableColumn("[bold]Subtotal[/]").RightAligned());

                    decimal total = 0;
                    foreach (var item in order.Items)
                    {
                        var productName = item.Product?.Name ?? "Unknown Product";
                        var subtotal = item.Quantity * item.UnitPrice;
                        total += subtotal;

                        itemsTable.AddRow(
                            Markup.Escape(productName),
                            item.Quantity.ToString(),
                            item.UnitPrice.ToString("C2"),
                            subtotal.ToString("C2")
                        );
                    }

                    itemsTable.AddEmptyRow();
                    itemsTable.AddRow(
                        "[bold]TOTAL[/]",
                        "",
                        "",
                        $"[bold]{total:C2}[/]"
                    );

                    AnsiConsole.Write(itemsTable);
                    AnsiConsole.WriteLine();
                }

                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error retrieving order details: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task CreateNewOrderWizardAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Order Management > Create New Order");
            UIHelper.ShowHeader("CREATE NEW ORDER - WIZARD");

            AnsiConsole.MarkupLine("[cyan]This wizard will guide you through creating a new order.[/]");
            AnsiConsole.MarkupLine("[grey]Steps: [1] Select Customer → [2] Add Products → [3] Review → [4] Confirm[/]");
            AnsiConsole.WriteLine();

            if (!UIHelper.Confirm("Start order creation?"))
            {
                return;
            }

            try
            {
                // Step 1: Select Customer
                var customer = await SelectCustomerAsync();
                if (customer == null) return;

                // Step 2: Add Products
                var orderItems = await AddProductsToOrderAsync();
                if (orderItems == null || !orderItems.Any()) return;

                // Step 3: Review Order
                if (!await ReviewOrderAsync(customer, orderItems)) return;

                // Step 4: Create Order
                await CreateOrderAsync(customer, orderItems);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error creating order: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task<Customers?> SelectCustomerAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Order Management > Create New Order > Step 1: Select Customer");
            UIHelper.ShowHeader("STEP 1/4: SELECT CUSTOMER");

            var customers = await _customerService.GetAllAsync();
            
            if (!customers.Any())
            {
                UIHelper.ShowError("No customers found. Please create a customer first.");
                UIHelper.PressAnyKey();
                return null;
            }

            // Display customers
            var table = UIHelper.CreateDataTable();
            table.AddColumn(new TableColumn("[bold]ID[/]").RightAligned());
            table.AddColumn("[bold]Name[/]");
            table.AddColumn("[bold]Email[/]");

            foreach (var c in customers.OrderBy(c => c.LastName))
            {
                table.AddRow(
                    UIHelper.FormatGuid(c.CustomerId),
                    $"{Markup.Escape(c.FirstName)} {Markup.Escape(c.LastName)}",
                    Markup.Escape(c.Email)
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            var customerIdStr = UIHelper.PromptString("Enter Customer ID (first 8 characters) or 0 to cancel:");
            
            if (customerIdStr == "0") return null;

            var customer = customers.FirstOrDefault(c => 
                c.CustomerId.ToString().StartsWith(customerIdStr, StringComparison.OrdinalIgnoreCase));

            if (customer == null)
            {
                UIHelper.ShowError("Customer not found.");
                UIHelper.PressAnyKey();
                return null;
            }

            UIHelper.ShowSuccess($"Selected customer: {customer.FirstName} {customer.LastName}");
            UIHelper.PressAnyKey();
            return customer;
        }

        private async Task<List<OrderItem>?> AddProductsToOrderAsync()
        {
            var orderItems = new List<OrderItem>();
            var products = await _productService.GetAllAsync();

            if (!products.Any())
            {
                UIHelper.ShowError("No products found. Please create products first.");
                UIHelper.PressAnyKey();
                return null;
            }

            bool addingProducts = true;
            while (addingProducts)
            {
                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Order Management > Create New Order > Step 2: Add Products");
                UIHelper.ShowHeader($"STEP 2/4: ADD PRODUCTS ({orderItems.Count} items)");

                // Show available products
                var table = UIHelper.CreateDataTable();
                table.AddColumn(new TableColumn("[bold]ID[/]").RightAligned());
                table.AddColumn("[bold]Product Name[/]");
                table.AddColumn(new TableColumn("[bold]Price[/]").RightAligned());

                foreach (var p in products.OrderBy(p => p.Name))
                {
                    table.AddRow(
                        UIHelper.FormatGuid(p.ProductId),
                        Markup.Escape(p.Name),
                        p.Price.ToString("C2")
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();

                // Show current cart
                if (orderItems.Any())
                {
                    var cartTable = UIHelper.CreateDataTable();
                    cartTable.Title = new TableTitle("[bold green]CURRENT CART[/]");
                    cartTable.AddColumn("[bold]Product[/]");
                    cartTable.AddColumn(new TableColumn("[bold]Qty[/]").RightAligned());
                    cartTable.AddColumn(new TableColumn("[bold]Subtotal[/]").RightAligned());

                    foreach (var item in orderItems)
                    {
                        var productInCart = products.First(p => p.ProductId == item.ProductId);
                        cartTable.AddRow(
                            Markup.Escape(productInCart.Name),
                            item.Quantity.ToString(),
                            (item.Quantity * item.UnitPrice).ToString("C2")
                        );
                    }

                    AnsiConsole.Write(cartTable);
                    AnsiConsole.WriteLine();
                }

                var productIdStr = UIHelper.PromptString("Enter Product ID to add (or 0 to finish):");
                
                if (productIdStr == "0")
                {
                    if (orderItems.Any())
                    {
                        addingProducts = false;
                    }
                    else
                    {
                        UIHelper.ShowWarning("Please add at least one product.");
                        UIHelper.PressAnyKey();
                    }
                    continue;
                }

                var product = products.FirstOrDefault(p => 
                    p.ProductId.ToString().StartsWith(productIdStr, StringComparison.OrdinalIgnoreCase));

                if (product == null)
                {
                    UIHelper.ShowError("Product not found.");
                    UIHelper.PressAnyKey();
                    continue;
                }

                var quantity = UIHelper.PromptInteger($"Quantity for {product.Name}:", 1);

                orderItems.Add(new OrderItem
                {
                    ProductId = product.ProductId,
                    Product = product,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });

                UIHelper.ShowSuccess($"Added {quantity}x {product.Name}");
                UIHelper.PressAnyKey();
            }

            return orderItems;
        }

        private Task<bool> ReviewOrderAsync(Customers customer, List<OrderItem> orderItems)
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Order Management > Create New Order > Step 3: Review Order");
            UIHelper.ShowHeader("STEP 3/4: REVIEW ORDER");

            // Customer info
            var customerInfo = $"[bold]Customer:[/] {Markup.Escape(customer.FirstName)} {Markup.Escape(customer.LastName)}\n" +
                             $"[bold]Email:[/] {Markup.Escape(customer.Email)}";
            UIHelper.ShowPanel("CUSTOMER INFORMATION", customerInfo, Color.Blue);

            // Order items
            var itemsTable = UIHelper.CreateDataTable();
            itemsTable.Title = new TableTitle("[bold]ORDER ITEMS[/]");
            itemsTable.AddColumn("[bold]Product[/]");
            itemsTable.AddColumn(new TableColumn("[bold]Qty[/]").RightAligned());
            itemsTable.AddColumn(new TableColumn("[bold]Unit Price[/]").RightAligned());
            itemsTable.AddColumn(new TableColumn("[bold]Subtotal[/]").RightAligned());

            decimal total = 0;
            foreach (var item in orderItems)
            {
                var subtotal = item.Quantity * item.UnitPrice;
                total += subtotal;

                itemsTable.AddRow(
                    Markup.Escape(item.Product?.Name ?? "Unknown"),
                    item.Quantity.ToString(),
                    item.UnitPrice.ToString("C2"),
                    subtotal.ToString("C2")
                );
            }

            itemsTable.AddEmptyRow();
            itemsTable.AddRow(
                "[bold]TOTAL[/]",
                "",
                "",
                $"[bold green]{total:C2}[/]"
            );

            AnsiConsole.Write(itemsTable);
            AnsiConsole.WriteLine();

            return Task.FromResult(UIHelper.Confirm("Confirm and create this order?"));
        }

        private async Task CreateOrderAsync(Customers customer, List<OrderItem> orderItems)
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Order Management > Create New Order > Step 4: Confirm");
            UIHelper.ShowHeader("STEP 4/4: CREATING ORDER");

            // Convert OrderItems to OrderItemDto
            var itemDtos = orderItems.Select(item => new OrderItemDto(
                item.ProductId,
                item.Quantity,
                item.UnitPrice
            )).ToList();

            var order = await UIHelper.WithSpinnerAsync(
                "Creating order...",
                async () => await _orderService.CreateOrderAsync(customer.CustomerId, itemDtos)
            );

            UIHelper.ShowSuccess($"Order created successfully! Order ID: {UIHelper.FormatGuid(order.OrderId)}");
            
            var total = orderItems.Sum(i => i.Quantity * i.UnitPrice);
            AnsiConsole.MarkupLine($"[cyan]Total: {total:C2}[/]");
            AnsiConsole.MarkupLine($"[cyan]Items: {orderItems.Count}[/]");
            
            UIHelper.PressAnyKey();
        }

        private async Task DeleteOrderAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Order Management > Delete Order");
            UIHelper.ShowHeader("DELETE ORDER");

            var orderIdStr = UIHelper.PromptString("Enter Order ID (first 8 characters) or 0 to cancel:");
            
            if (orderIdStr == "0") return;

            try
            {
                var allOrders = await _orderService.GetAllAsync();
                var order = allOrders.FirstOrDefault(o => 
                    o.OrderId.ToString().StartsWith(orderIdStr, StringComparison.OrdinalIgnoreCase));

                if (order == null)
                {
                    UIHelper.ShowError("Order not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                var customerName = order.Customer != null 
                    ? $"{order.Customer.FirstName} {order.Customer.LastName}"
                    : "Unknown";
                
                var total = order.Items.Sum(i => i.Quantity * i.UnitPrice);
                var orderInfo = $"Order {UIHelper.FormatGuid(order.OrderId)} for {customerName} ({total:C2}, {order.Items.Count} items)";
                
                if (!UIHelper.ConfirmDeletion(orderInfo))
                {
                    UIHelper.ShowWarning("Deletion cancelled.");
                    UIHelper.PressAnyKey();
                    return;
                }

                await UIHelper.WithSpinnerAsync(
                    "Deleting order...",
                    async () => await _orderService.DeleteOrderAsync(order.OrderId)
                );

                UIHelper.ShowSuccess("Order deleted successfully!");
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error deleting order: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }
    }
}
