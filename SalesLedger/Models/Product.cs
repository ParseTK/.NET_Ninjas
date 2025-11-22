using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesLedger.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

