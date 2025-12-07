using SalesLedger.Infrastructure.Data;
using SalesLedger.Domain;

namespace SalesLedger.Infrastructure.Repositories;
public class ProductRepository(SalesLedgerDbContext context)
    : Repository<Products>(context), IProductRepository
{
}