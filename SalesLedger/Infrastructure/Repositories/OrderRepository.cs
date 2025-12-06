using Microsoft.EntityFrameworkCore;
using SalesLedger.Infrastructure.Data;
using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Repositories;
public class OrderRepository(SalesLedgerDbContext context)
    : Repository<Orders>(context), IOrderRepository
{
    public async Task<Orders?> GetByIdWithItemsAsync(Guid orderId, CancellationToken ct = default)
        => await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items!)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId, ct);

    public async Task<IReadOnlyCollection<Orders>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .ToListAsync(ct);
}
