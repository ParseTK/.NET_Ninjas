using Microsoft.EntityFrameworkCore;
using SalesLedger.Domain;

namespace SalesLedger.Data
{
    public class SalesLedgerDbContext : DbContext
    {
        public SalesLedgerDbContext(DbContextOptions<SalesLedgerDbContext> options) : 
            base(options) { }

        public DbSet<Customers> Customers { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
    }
}
