using Microsoft.EntityFrameworkCore;
using SalesLedger.Application.Interfaces;
using SalesLedger.Infrastructure.Data;
using SalesLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesLedger.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly SalesLedgerDbContext _db;

        public CustomerService(SalesLedgerDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Customers> CreateAsync(Customers customer, CancellationToken cancellationToken = default)
        {
            await _db.Customers.AddAsync(customer, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return customer;
        }

        public async Task<Customers?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Customers.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Customers>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Customers.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Customers customer, CancellationToken cancellationToken = default)
        {
            var existing = await _db.Customers.FindAsync(new object[] { customer.Id }, cancellationToken);
            if (existing is null) return false;

            _db.Entry(existing).CurrentValues.SetValues(customer);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existing = await _db.Customers.FindAsync(new object[] { id }, cancellationToken);
            if (existing is null) return false;

            _db.Customers.Remove(existing);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
