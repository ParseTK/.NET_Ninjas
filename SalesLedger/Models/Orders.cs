using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesLedger.Models
{
    public class Orders
    {
        [Key]
        public Guid OrderId { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public Customers? Customer { get; set; }
        public Products? Product { get; set; }

    }
}
