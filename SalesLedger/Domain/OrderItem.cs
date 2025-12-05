namespace SalesLedger.Domain
{
    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Orders Order { get; set; } = null!;
        public Products Product { get; set; } = null!;
    }
}
