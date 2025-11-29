using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesLedger.Data;
using SalesLedger.Interfaces;
using SalesLedger.Models;

namespace SalesLedger.Services
{
    public class OrderService : IOrderService
    {
        private readonly SalesLedgerDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(SalesLedgerDbContext context, ILogger<OrderService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Orders> CreateAsync(Orders order, CancellationToken cancellationToken = default)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderId == Guid.Empty)
                order.OrderId = Guid.NewGuid();

            ValidateOrder(order);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created order with ID: {OrderId}", order.OrderId);
            return order;
        }

        public async Task<Orders?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id, cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty)
                return Enumerable.Empty<Orders>();

            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Orders>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Enumerable.Empty<Orders>();

            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .Where(o => o.ProductId == productId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Orders order, CancellationToken cancellationToken = default)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderId == Guid.Empty)
                return false;

            ValidateOrder(order);

            var existing = await _context.Orders
                .FindAsync(new object[] { order.OrderId }, cancellationToken);

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(order);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated order with ID: {OrderId}", order.OrderId);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return false;

            var order = await _context.Orders
                .FindAsync(new object[] { id }, cancellationToken);

            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted order with ID: {OrderId}", id);
            return true;
        }

        private void ValidateOrder(Orders order)
        {
            var errors = new List<string>();

            if (order.CustomerId == Guid.Empty)
                errors.Add("Customer ID is required");

            if (order.ProductId == Guid.Empty)
                errors.Add("Product ID is required");

            if (order.Quantity <= 0)
                errors.Add("Quantity must be greater than zero");

            if (order.UnitPrice < 0)
                errors.Add("Unit price cannot be negative");

            if (order.OrderDate == default)
                errors.Add("Order date is required");

            if (errors.Any())
            {
                var errorMessage = string.Join("; ", errors);
                throw new InvalidOperationException($"Order validation failed: {errorMessage}");
            }
        }
    }
}