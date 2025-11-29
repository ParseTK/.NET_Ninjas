using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;



namespace SalesLedger.Models
{
    public class Customers
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Orders> Orders { get; set; } = new List<Orders>();
    }
}
