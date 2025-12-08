using Spectre.Console;

namespace SalesLedger.UI
{
    /// Common UI helper methods for consistent display across the application
    public static class UIHelper
    {
        /// Displays the breadcrumb navigation at the top of screen
        public static void ShowBreadcrumb(string path)
        {
            AnsiConsole.MarkupLine($"[silver]SALES LEDGER > {path}[/]");
            AnsiConsole.WriteLine();
        }

        /// Displays a section header with rounded border
        public static void ShowHeader(string title)
        {
            var headerPanel = new Panel(new Markup($"[bold cyan]{title}[/]"))
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0),
                BorderStyle = new Style(Color.Blue)
            };
            AnsiConsole.Write(headerPanel);
            AnsiConsole.WriteLine();
        }

        /// Displays a success message
        public static void ShowSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[green]✓ {message}[/]");
        }

        /// Displays an error message
        public static void ShowError(string message)
        {
            AnsiConsole.MarkupLine($"[red]✗ {message}[/]");
        }

        /// Displays a warning message
        public static void ShowWarning(string message)
        {
            AnsiConsole.MarkupLine($"[yellow]❢ {message}[/]");
        }

        /// Displays an info message
        public static void ShowInfo(string message)
        {
            AnsiConsole.MarkupLine($"[grey]{message}[/]");
        }

        /// Prompts user to press any key to continue
        public static void PressAnyKey(string message = "Press any key to continue...")
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[grey]{message}[/]");
            Console.ReadKey(true);
        }

        /// Clears the screen and optionally shows header
        public static void ClearScreen()
        {
            Console.Clear();
        }

        /// Creates a menu table with rounded border
        public static Table CreateMenuTable(string? title = null)
        {
            var table = new Table().HideHeaders();
            table.AddColumn(new TableColumn("Option"));
            table.AddColumn(new TableColumn("Description"));
            table.Border = TableBorder.Rounded;
            
            if (!string.IsNullOrWhiteSpace(title))
            {
                table.Title = new TableTitle($"[bold yellow]{title}[/]");
            }
            
            return table;
        }

        /// Prompts for a validated menu choice
        public static char PromptMenuChoice(string prompt, Func<char, bool> validator, string errorMessage = "Invalid choice. Please try again.")
        {
            return AnsiConsole.Prompt(
                new TextPrompt<char>(prompt)
                    .PromptStyle("yellow")
                    .Validate(c =>
                    {
                        if (validator(c))
                            return ValidationResult.Success();
                        return ValidationResult.Error($"[red]{errorMessage}[/]");
                    })
            );
        }

        /// Prompts for a string with validation
        public static string PromptString(string prompt, Func<string, ValidationResult>? validator = null)
        {
            var textPrompt = new TextPrompt<string>(prompt)
                .PromptStyle("cyan");

            if (validator != null)
            {
                textPrompt.Validate(validator);
            }

            return AnsiConsole.Prompt(textPrompt);
        }

        /// Prompts for a required string
        public static string PromptRequiredString(string fieldName, int maxLength = 100)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"{fieldName}:")
                    .PromptStyle("cyan")
                    .Validate(value =>
                    {
                        if (string.IsNullOrWhiteSpace(value))
                            return ValidationResult.Error($"{fieldName} is required");
                        if (value.Length > maxLength)
                            return ValidationResult.Error($"{fieldName} cannot exceed {maxLength} characters");
                        return ValidationResult.Success();
                    })
            );
        }

        /// Prompts for an email address
        public static string PromptEmail()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>("Email:")
                    .PromptStyle("cyan")
                    .Validate(email =>
                    {
                        if (string.IsNullOrWhiteSpace(email))
                            return ValidationResult.Error("Email is required");
                        if (!email.Contains('@') || email.Length < 5)
                            return ValidationResult.Error("Please enter a valid email address");
                        return ValidationResult.Success();
                    })
            );
        }

        /// Prompts for a decimal value such as price
        public static decimal PromptDecimal(string prompt, decimal min = 0)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<decimal>(prompt)
                    .PromptStyle("cyan")
                    .Validate(value =>
                    {
                        if (value < min)
                            return ValidationResult.Error($"Value must be at least {min:C}");
                        return ValidationResult.Success();
                    })
            );
        }

        /// Prompts for a positive integer
        public static int PromptInteger(string prompt, int min = 1)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<int>(prompt)
                    .PromptStyle("cyan")
                    .Validate(value =>
                    {
                        if (value < min)
                            return ValidationResult.Error($"Value must be at least {min}");
                        return ValidationResult.Success();
                    })
            );
        }

        /// Shows a confirmation prompt
        public static bool Confirm(string question)
        {
            return AnsiConsole.Confirm(question);
        }

        /// Shows a dangerous action confirmation requiring typing "DELETE"
        public static bool ConfirmDeletion(string itemDescription)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[red]You are about to delete: {Markup.Escape(itemDescription)}[/]");
            AnsiConsole.MarkupLine("[yellow]This action cannot be undone.[/]");
            AnsiConsole.WriteLine();
            
            var confirmation = AnsiConsole.Prompt(
                new TextPrompt<string>("Type [red]DELETE[/] to confirm, or anything else to cancel:")
                    .AllowEmpty()
            );
            
            return confirmation == "DELETE";
        }

        /// Creates a data table for displaying records
        public static Table CreateDataTable()
        {
            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.BorderStyle = new Style(Color.Grey);
            return table;
        }

        /// Shows a loading spinner while executing an async operation
        public static async Task<T> WithSpinnerAsync<T>(string message, Func<Task<T>> operation)
        {
            return await AnsiConsole.Status()
                .StartAsync(message, async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    ctx.SpinnerStyle(Style.Parse("cyan"));
                    return await operation();
                });
        }

        /// Shows a loading spinner for void operations
        public static async Task WithSpinnerAsync(string message, Func<Task> operation)
        {
            await AnsiConsole.Status()
                .StartAsync(message, async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    ctx.SpinnerStyle(Style.Parse("cyan"));
                    await operation();
                });
        }

        /// Displays a panel with information
        public static void ShowPanel(string title, string content, Color borderColor)
        {
            var panel = new Panel(content)
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(borderColor),
                Padding = new Padding(1, 0)
            }.Header(title, Justify.Left);
            
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        /// Truncates text to a maximum length with ellipsis
        public static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
            return text.Substring(0, maxLength - 3) + "...";
        }

        /// Formats a GUID for display (shows first 8 characters)
        public static string FormatGuid(Guid guid)
        {
            return guid.ToString().Substring(0, 8);
        }
    }
}
