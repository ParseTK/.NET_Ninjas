using SalesLedger.Application.Interfaces;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Repositories;

namespace SalesLedger.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<Orders?> GetByIdAsync(Guid orderId, CancellationToken ct = default)
        => await _orderRepository.GetByIdAsync(orderId, ct);

    public async Task<IReadOnlyCollection<Orders>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await _orderRepository.GetByCustomerIdAsync(customerId, ct);

    public async Task<IReadOnlyCollection<Orders>> GetAllAsync(CancellationToken ct = default)
        => await _orderRepository.GetAllAsync(ct);

    public async Task<Orders> CreateOrderAsync(Guid customerId, IReadOnlyCollection<OrderItemDto> items, CancellationToken ct = default)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId, ct)
            ?? throw new KeyNotFoundException($"Customer {customerId} not found.");

        var order = new Orders
        {
            OrderId = Guid.NewGuid(),
            CustomerId = customerId,
            Customer = customer,
            OrderDate = DateTime.UtcNow,
            Items = []
        };

        foreach (var dto in items)
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId, ct)
                ?? throw new KeyNotFoundException($"Product {dto.ProductId} not found.");

            var unitPrice = dto.UnitPriceOverride ?? product.Price;

            order.Items.Add(new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = product.ProductId,
                Product = product,
                Quantity = dto.Quantity,
                UnitPrice = unitPrice
            });
        }

        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.UnitOfWork.SaveChangesAsync(ct);

        return order;
    }

    public async Task AddItemToOrderAsync(Guid orderId, Guid productId, int quantity, decimal? unitPriceOverride = null, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        var product = await _productRepository.GetByIdAsync(productId, ct)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        var unitPrice = unitPriceOverride ?? product.Price;
        var existing = order.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existing != null)
            existing.Quantity += quantity;
        else
            order.Items.Add(new OrderItem { OrderId = orderId, ProductId = productId, Product = product, Quantity = quantity, UnitPrice = unitPrice });

        await _orderRepository.UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveItemFromOrderAsync(Guid orderId, Guid productId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        var item = order.Items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new KeyNotFoundException($"Product {productId} not in order.");

        order.Items.Remove(item);
        await _orderRepository.UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        _orderRepository.Remove(order);
        await _orderRepository.UnitOfWork.SaveChangesAsync(ct);
    }
}