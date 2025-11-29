using Microsoft.EntityFrameworkCore;
using SalesLedger.Models;

namespace SalesLedger.Data
{
    public class SalesLedgerDbContext : DbContext
    {
        public SalesLedgerDbContext(DbContextOptions<SalesLedgerDbContext> options) : 
            base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
