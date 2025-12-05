using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesLedger.Domain
{
    public class Products
    {
        public Guid ProductId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

