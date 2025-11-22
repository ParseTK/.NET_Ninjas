using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesLedger.Models
{
    public class Product
    {
       
        public int ProductId { get; set; }

        
        public string ProductName { get; set; }

        public string Email { get; set; }

        public decimal UnitPrice { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

