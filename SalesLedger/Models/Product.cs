using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesLedger.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        [MaxLength(200)]
        public required string ProductName { get; set; }

        [Required]
        public required decimal UnitPrice { get; set; }

        public ICollection<Order> Orders { get; set; } = [];
    }
}

