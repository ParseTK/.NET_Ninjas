using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Repositories;
public interface ICustomerRepository : IRepository<Customers>
{
    Task<Customers?> GetWithOrdersAsync(Guid customerId, CancellationToken ct = default);
}
