// Application/Services/OrderItemService.cs
using Microsoft.EntityFrameworkCore;
using SalesLedger.Application.Interfaces;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Repositories;

namespace SalesLedger.Application.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IOrderRepository _orderRepository;

    public OrderItemService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderItem?> GetByKeyAsync(Guid orderId, Guid productId, CancellationToken ct = default)
    {
        return await _orderRepository.Query()
            .Where(o => o.OrderId == orderId)
            .SelectMany(o => o.Items!)
            .FirstOrDefaultAsync(i => i.ProductId == productId, ct);
    }

    public async Task<IReadOnlyCollection<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _orderRepository.Query()
            .Where(o => o.OrderId == orderId)
            .SelectMany(o => o.Items!)
            .ToListAsync(ct);
    }

    public Task<OrderItem> AddItemAsync(Guid orderId, Guid productId, int quantity, decimal? unitPriceOverride = null, CancellationToken ct = default)
        => throw new NotSupportedException("Use OrderService.AddItemToOrderAsync instead.");

    public Task UpdateQuantityAsync(Guid orderId, Guid productId, int newQuantity, CancellationToken ct = default)
        => throw new NotSupportedException("Use OrderService to modify order items.");

    public Task UpdateDiscountAsync(Guid orderId, Guid productId, decimal discount, CancellationToken ct = default)
        => throw new NotSupportedException("Use OrderService to modify order items.");

    public Task RemoveItemAsync(Guid orderId, Guid productId, CancellationToken ct = default)
        => throw new NotSupportedException("Use OrderService.RemoveItemFromOrderAsync instead.");
}