namespace SalesLedger.Infrastructure.Repositories;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}