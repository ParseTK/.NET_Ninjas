using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesLedger.Models
{
    public class Products
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        [MaxLength(200)]
        public required string ProductName { get; set; }

        [Required]
        public required decimal UnitPrice { get; set; }

        public ICollection<Orders> Orders { get; set; } = [];
    }
}

