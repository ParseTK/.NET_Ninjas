using SalesLedger.Models;

namespace SalesLedger.Application.Interfaces
{
       public interface IProductService
    {
        Task<Products> CreateAsync(Products product, CancellationToken cancellationToken = default);
        Task<Products?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Products>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Products product, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
