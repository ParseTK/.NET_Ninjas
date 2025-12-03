using SalesLedger.Models;

namespace SalesLedger.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Orders> CreateAsync(Orders order, CancellationToken cancellationToken = default);
        Task<Orders?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Orders>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Orders>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Orders>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Orders order, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}