using System.ComponentModel.DataAnnotations;



namespace SalesLedger.Models
{
    public class Customers
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public ICollection<Orders> Orders { get; set; } = [];
    }
}
