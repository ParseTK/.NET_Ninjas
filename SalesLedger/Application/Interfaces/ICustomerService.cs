using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesLedger.Models;

namespace SalesLedger.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Customers> CreateAsync(Customers customer, CancellationToken cancellationToken = default);
        Task<Customers?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Customers>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Customers customer, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
