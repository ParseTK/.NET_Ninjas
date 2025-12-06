using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Repositories;
public interface IOrderRepository : IRepository<Orders>
{
    Task<Orders?> GetByIdWithItemsAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyCollection<Orders>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
}
