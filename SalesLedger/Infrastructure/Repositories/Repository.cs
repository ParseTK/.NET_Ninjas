using Microsoft.EntityFrameworkCore;
using SalesLedger.Infrastructure.Data;
using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SalesLedgerDbContext _context;
    public IUnitOfWork UnitOfWork => (IUnitOfWork)_context;
    public Repository(SalesLedgerDbContext context)
    {
        _context = context;
    }
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<T>().FindAsync([id], ct);
    public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken ct = default)
        => await _context.Set<T>().ToListAsync(ct);
    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _context.Set<T>().AddAsync(entity, ct);
    public void Remove(T entity)
        => _context.Set<T>().Remove(entity);
    public IQueryable<T> Query() => _context.Set<T>().AsQueryable();
}
