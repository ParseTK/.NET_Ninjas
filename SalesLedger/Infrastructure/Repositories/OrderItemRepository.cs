using SalesLedger.Infrastructure.Data;
using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Repositories;
public class OrderItemRepository(SalesLedgerDbContext context)
    : Repository<OrderItem>(context), IOrderItemRepository
{
}
