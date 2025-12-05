using System.ComponentModel.DataAnnotations;
using SalesLedger.Domain;



namespace SalesLedger.Domain
{
    public class Customers
    {
        public Guid CustomerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }

        public ICollection<Orders> Orders { get; set; } = new List<Orders>();
    }
}
