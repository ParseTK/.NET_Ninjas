using SalesLedger.Domain;

namespace SalesLedger.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Orders?> GetByIdAsync(Guid orderId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Orders>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Orders>> GetAllAsync(CancellationToken ct = default);
        Task<Orders> CreateOrderAsync(Guid customerId, IReadOnlyCollection<OrderItemDto> items, CancellationToken ct = default);
        Task AddItemToOrderAsync(Guid orderId, Guid productId, int quantity, decimal? unitPriceOverride = null, CancellationToken ct = default);
        Task RemoveItemFromOrderAsync(Guid orderId, Guid productId, CancellationToken ct = default);
        Task DeleteOrderAsync(Guid orderId, CancellationToken ct = default);
    }

    public record OrderItemDto(Guid ProductId, int Quantity, decimal? UnitPriceOverride = null);
}