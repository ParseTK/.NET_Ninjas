using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesLedger.Models;

namespace SalesLedger.Interfaces
{
    public interface IOrderService
    {
        Task<Order> AddAsync(Order order);
        Task<Order> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> UpdateAsync(Order order);

        Task<Order> DeleteAsync(int id);
    }
}
