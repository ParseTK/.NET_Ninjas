using SalesLedger.Domain;

namespace SalesLedger.Application.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItem?> GetByKeyAsync(Guid orderId, Guid productId, CancellationToken ct = default);
        Task<IReadOnlyCollection<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
        Task<OrderItem> AddItemAsync(Guid orderId, Guid productId, int quantity, decimal? unitPriceOverride = null, CancellationToken ct = default);
        Task UpdateQuantityAsync(Guid orderId, Guid productId, int newQuantity, CancellationToken ct = default);
        Task UpdateDiscountAsync(Guid orderId, Guid productId, decimal discount, CancellationToken ct = default);
        Task RemoveItemAsync(Guid orderId, Guid productId, CancellationToken ct = default);
    }
}
