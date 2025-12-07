using SalesLedger.Domain;

namespace SalesLedger.Domain
{
    public class Orders
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public Guid CustomerId { get; set; }

        public Customers Customer { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
