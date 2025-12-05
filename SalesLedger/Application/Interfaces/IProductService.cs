using SalesLedger.Domain;

namespace SalesLedger.Application.Interfaces
{
    public interface IProductService
    {
        Task<Products?> GetByIdAsync(Guid productId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Products>> GetAllAsync(CancellationToken ct = default);
        Task<Products> CreateAsync(Products product, CancellationToken ct = default);
        Task UpdateAsync(Products product, CancellationToken ct = default);
        Task DeleteAsync(Guid productId, CancellationToken ct = default);
    }
}
