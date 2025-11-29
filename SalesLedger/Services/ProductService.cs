using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesLedger.Data;
using SalesLedger.Interfaces;
using SalesLedger.Models;

namespace SalesLedger.Services
{
    public class ProductService : IProductService
    {
        private readonly SalesLedgerDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(SalesLedgerDbContext context, ILogger<ProductService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.ProductId == Guid.Empty)
                product.ProductId = Guid.NewGuid();

            ValidateProduct(product);

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created product with ID: {ProductId}", product.ProductId);
            return product;
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.ProductName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.ProductId == Guid.Empty)
                return false;

            ValidateProduct(product);

            var existing = await _context.Products
                .FindAsync(new object[] { product.ProductId }, cancellationToken);

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated product with ID: {ProductId}", product.ProductId);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return false;

            var product = await _context.Products
                .FindAsync(new object[] { id }, cancellationToken);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted product with ID: {ProductId}", id);
            return true;
        }

        private void ValidateProduct(Product product)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(product.ProductName))
                errors.Add("Product name is required");

            if (product.ProductName?.Length > 200)
                errors.Add("Product name cannot exceed 200 characters");

            if (product.UnitPrice < 0)
                errors.Add("Product price cannot be negative");

            if (errors.Any())
            {
                var errorMessage = string.Join("; ", errors);
                throw new InvalidOperationException($"Product validation failed: {errorMessage}");
            }
        }
    }
}