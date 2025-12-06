using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Repositories;
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Remove(T entity);
    IQueryable<T> Query();
    IUnitOfWork UnitOfWork { get; }
}
