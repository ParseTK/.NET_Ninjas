using SalesLedger.Domain;

namespace SalesLedger.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Customers?> GetByIdAsync(Guid customerId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Customers>> GetAllAsync(CancellationToken ct = default);
        Task<Customers> CreateAsync(Customers customer, CancellationToken ct = default);
        Task UpdateAsync(Customers customer, CancellationToken ct = default);
        Task DeleteAsync(Guid customerId, CancellationToken ct = default);
        Task<Customers?> GetWithOrdersAsync(Guid customerId, CancellationToken ct = default);
    }
}
