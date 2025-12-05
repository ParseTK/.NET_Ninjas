using Microsoft.EntityFrameworkCore;
using SalesLedger.Data;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Data;

namespace SalesLedger.Infrastructure.Repositories;
public class CustomerRepository(SalesLedgerDbContext context)
    : Repository<Customers>(context), ICustomerRepository
{
    public async Task<Customers?> GetWithOrdersAsync(Guid customerId, CancellationToken ct = default)
        => await _context.Customers
            .Include(c => c.Orders!)
                .ThenInclude(o => o.Items!)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);
}