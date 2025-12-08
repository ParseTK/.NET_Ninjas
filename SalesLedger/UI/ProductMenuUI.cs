using Spectre.Console;
using SalesLedger.Application.Interfaces;
using SalesLedger.Domain;

namespace SalesLedger.UI
{
    /// Product Management UI screens
    public class ProductMenuUI
    {
        private readonly IProductService _productService;

        public ProductMenuUI(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ShowAsync()
        {
            bool backToMain = false;

            while (!backToMain)
            {
                UIHelper.ClearScreen();
                DisplayProductMenu();

                var choice = UIHelper.PromptMenuChoice(
                    "Enter your choice [[1-5, 0, Q]]:",
                    c => (c >= '0' && c <= '5') || char.ToUpper(c) == 'Q',
                    "Please enter 0-5 or Q"
                );

                switch (choice)
                {
                    case '1':
                        await ViewAllProductsAsync();
                        break;
                    case '2':
                        await SearchProductAsync();
                        break;
                    case '3':
                        await AddNewProductAsync();
                        break;
                    case '4':
                        await EditProductAsync();
                        break;
                    case '5':
                        await DeleteProductAsync();
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

        private void DisplayProductMenu()
        {
            UIHelper.ShowBreadcrumb("Product Management");
            UIHelper.ShowHeader("PRODUCT MANAGEMENT");

            // View & Search section
            var viewTable = UIHelper.CreateMenuTable("VIEW & SEARCH");
            viewTable.AddRow("[yellow]1.[/] View All Products", "Display complete product catalog");
            viewTable.AddRow("[yellow]2.[/] Search Product", "Find by name or ID");
            AnsiConsole.Write(viewTable);
            AnsiConsole.WriteLine();

            // Modify Records section
            var modifyTable = UIHelper.CreateMenuTable("MODIFY RECORDS");
            modifyTable.AddRow("[yellow]3.[/] Add New Product", "Create new product record");
            modifyTable.AddRow("[yellow]4.[/] Edit Product", "Update product information");
            modifyTable.AddRow("[yellow]5.[/] Delete Product", "Remove product (if not in orders)");
            AnsiConsole.Write(modifyTable);
            AnsiConsole.WriteLine();

            // Navigation section
            var navTable = UIHelper.CreateMenuTable("NAVIGATION");
            navTable.AddRow("[yellow]0.[/] Back to Main Menu", "");
            navTable.AddRow("[yellow]Q.[/] Exit Application", "");
            AnsiConsole.Write(navTable);
            AnsiConsole.WriteLine();
        }

        private async Task ViewAllProductsAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Product Management > View All Products");
            UIHelper.ShowHeader("PRODUCT CATALOG");

            try
            {
                var products = await UIHelper.WithSpinnerAsync(
                    "Loading products...",
                    async () => await _productService.GetAllAsync()
                );

                if (!products.Any())
                {
                    UIHelper.ShowWarning("No products found in the database.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Display count
                AnsiConsole.MarkupLine($"[grey]Total Products: {products.Count}[/]");
                AnsiConsole.WriteLine();

                // Create data table
                var table = UIHelper.CreateDataTable();
                table.AddColumn(new TableColumn("[bold]ID[/]").RightAligned());
                table.AddColumn("[bold]Product Name[/]");
                table.AddColumn(new TableColumn("[bold]Price[/]").RightAligned());
                table.AddColumn(new TableColumn("[bold]In Orders[/]").RightAligned());

                foreach (var product in products.OrderBy(p => p.Name))
                {
                    table.AddRow(
                        UIHelper.FormatGuid(product.ProductId),
                        Markup.Escape(product.Name),
                        product.Price.ToString("C2"),
                        product.OrderItems.Count.ToString()
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();

                // Actions
                UIHelper.ShowInfo("Actions: [[V]] View Details | [[E]] Edit | [[D]] Delete | [[0]] Back");
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
                        await ViewProductDetailsAsync();
                        break;
                    case 'E':
                        await EditProductAsync();
                        break;
                    case 'D':
                        await DeleteProductAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error loading products: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task ViewProductDetailsAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Product Management > View Product Details");

            var productIdStr = UIHelper.PromptString("Enter Product ID (first 8 characters) or 0 to cancel:");
            
            if (productIdStr == "0") return;

            try
            {
                var allProducts = await _productService.GetAllAsync();
                var product = allProducts.FirstOrDefault(p => 
                    p.ProductId.ToString().StartsWith(productIdStr, StringComparison.OrdinalIgnoreCase));

                if (product == null)
                {
                    UIHelper.ShowError("Product not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Product Management > View Product Details");
                UIHelper.ShowHeader($"PRODUCT DETAILS - {product.Name}");

                // Product info panel
                var infoContent = $"[bold]Product ID:[/] {UIHelper.FormatGuid(product.ProductId)}\n" +
                                $"[bold]Name:[/] {Markup.Escape(product.Name)}\n" +
                                $"[bold]Price:[/] {product.Price:C2}\n" +
                                $"[bold]Times Ordered:[/] {product.OrderItems.Count}";

                UIHelper.ShowPanel("PRODUCT INFORMATION", infoContent, Color.Blue);
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error retrieving product details: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task SearchProductAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Product Management > Search Product");
            UIHelper.ShowHeader("SEARCH PRODUCT");

            var searchTerm = UIHelper.PromptString("Enter product name (or part of it) to search:");
            
            if (string.IsNullOrWhiteSpace(searchTerm)) return;

            try
            {
                var allProducts = await _productService.GetAllAsync();
                var results = allProducts.Where(p => 
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!results.Any())
                {
                    UIHelper.ShowWarning("No products found matching your search.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Product Management > Search Results");
                UIHelper.ShowHeader($"SEARCH RESULTS ({results.Count} found)");

                var table = UIHelper.CreateDataTable();
                table.AddColumn(new TableColumn("[bold]ID[/]").RightAligned());
                table.AddColumn("[bold]Product Name[/]");
                table.AddColumn(new TableColumn("[bold]Price[/]").RightAligned());

                foreach (var product in results)
                {
                    table.AddRow(
                        UIHelper.FormatGuid(product.ProductId),
                        Markup.Escape(product.Name),
                        product.Price.ToString("C2")
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

        private async Task AddNewProductAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Product Management > Add New Product");
            UIHelper.ShowHeader("CREATE NEW PRODUCT");

            // Show validation rules
            var rulesText = "✓ Product name is required\n" +
                          "✓ Price must be greater than 0\n" +
                          "✓ Product name should be unique";
            UIHelper.ShowPanel("VALIDATION RULES", rulesText, Color.Yellow);

            try
            {
                // Collect input
                var name = UIHelper.PromptRequiredString("Product Name", 200);
                var price = UIHelper.PromptDecimal("Price ($):", 0.01m);

                // Confirm
                AnsiConsole.WriteLine();
                var confirmContent = $"Product Name: {Markup.Escape(name)}\n" +
                                   $"Price: {price:C2}";
                UIHelper.ShowPanel("CONFIRM NEW PRODUCT", confirmContent, Color.Green);

                if (!UIHelper.Confirm("Create this product?"))
                {
                    UIHelper.ShowWarning("Product creation cancelled.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Create product
                var product = new Products
                {
                    ProductId = Guid.NewGuid(),
                    Name = name,
                    Price = price
                };

                await UIHelper.WithSpinnerAsync(
                    "Creating product...",
                    async () => await _productService.CreateAsync(product)
                );

                UIHelper.ShowSuccess($"Product '{name}' created successfully!");
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error creating product: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task EditProductAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Product Management > Edit Product");

            var productIdStr = UIHelper.PromptString("Enter Product ID (first 8 characters) or 0 to cancel:");
            
            if (productIdStr == "0") return;

            try
            {
                var allProducts = await _productService.GetAllAsync();
                var product = allProducts.FirstOrDefault(p => 
                    p.ProductId.ToString().StartsWith(productIdStr, StringComparison.OrdinalIgnoreCase));

                if (product == null)
                {
                    UIHelper.ShowError("Product not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                UIHelper.ClearScreen();
                UIHelper.ShowBreadcrumb("Product Management > Edit Product");
                UIHelper.ShowHeader($"EDIT PRODUCT - {product.Name}");

                // Show current data
                var currentData = $"Current Name: {Markup.Escape(product.Name)}\n" +
                                $"Current Price: {product.Price:C2}";
                UIHelper.ShowPanel("CURRENT INFORMATION", currentData, Color.Grey);

                // Get new values
                AnsiConsole.MarkupLine("[grey]Press Enter to keep current value[/]");
                AnsiConsole.WriteLine();

                var name = AnsiConsole.Prompt(
                    new TextPrompt<string>("Product Name:")
                        .DefaultValue(product.Name)
                        .AllowEmpty()
                ) ?? product.Name;

                var price = AnsiConsole.Prompt(
                    new TextPrompt<decimal>("Price ($):")
                        .DefaultValue(product.Price)
                        .Validate(p =>
                        {
                            if (p <= 0)
                                return ValidationResult.Error("Price must be greater than 0");
                            return ValidationResult.Success();
                        })
                );

                // Confirm changes
                if (!UIHelper.Confirm("Save changes?"))
                {
                    UIHelper.ShowWarning("Edit cancelled.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Update
                product.Name = name;
                product.Price = price;

                await UIHelper.WithSpinnerAsync(
                    "Updating product...",
                    async () => await _productService.UpdateAsync(product)
                );

                UIHelper.ShowSuccess("Product updated successfully!");
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error updating product: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }

        private async Task DeleteProductAsync()
        {
            UIHelper.ClearScreen();
            UIHelper.ShowBreadcrumb("Product Management > Delete Product");
            UIHelper.ShowHeader("DELETE PRODUCT");

            var productIdStr = UIHelper.PromptString("Enter Product ID (first 8 characters) or 0 to cancel:");
            
            if (productIdStr == "0") return;

            try
            {
                var allProducts = await _productService.GetAllAsync();
                var product = allProducts.FirstOrDefault(p => 
                    p.ProductId.ToString().StartsWith(productIdStr, StringComparison.OrdinalIgnoreCase));

                if (product == null)
                {
                    UIHelper.ShowError("Product not found.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Check if product is in any orders
                if (product.OrderItems.Any())
                {
                    UIHelper.ShowError($"Cannot delete product that appears in orders (Used in {product.OrderItems.Count} order(s))");
                    UIHelper.ShowInfo("Products that have been ordered cannot be deleted to maintain data integrity.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Show product info and confirm deletion
                var productInfo = $"{product.Name} ({product.Price:C2})";
                
                if (!UIHelper.ConfirmDeletion(productInfo))
                {
                    UIHelper.ShowWarning("Deletion cancelled.");
                    UIHelper.PressAnyKey();
                    return;
                }

                // Delete
                await UIHelper.WithSpinnerAsync(
                    "Deleting product...",
                    async () => await _productService.DeleteAsync(product.ProductId)
                );

                UIHelper.ShowSuccess("Product deleted successfully!");
                UIHelper.PressAnyKey();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error deleting product: {ex.Message}");
                UIHelper.PressAnyKey();
            }
        }
    }
}
