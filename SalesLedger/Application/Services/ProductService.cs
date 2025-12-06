using SalesLedger.Application.Interfaces;
using SalesLedger.Domain;
using SalesLedger.Infrastructure.Repositories;

namespace SalesLedger.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Products?> GetByIdAsync(Guid productId, CancellationToken ct = default)
        => await _repository.GetByIdAsync(productId, ct);

    public async Task<IReadOnlyCollection<Products>> GetAllAsync(CancellationToken ct = default)
        => await _repository.GetAllAsync(ct);

    public async Task<Products> CreateAsync(Products product, CancellationToken ct = default)
    {
        await _repository.AddAsync(product, ct);
        await _repository.UnitOfWork.SaveChangesAsync(ct);
        return product;
    }

    public async Task UpdateAsync(Products product, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(product.ProductId, ct)
            ?? throw new KeyNotFoundException($"Product {product.ProductId} not found.");

        existing.Name = product.Name;
        existing.Price = product.Price;

        await _repository.UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(productId, ct)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        _repository.Remove(product);
        await _repository.UnitOfWork.SaveChangesAsync(ct);
    }
}
