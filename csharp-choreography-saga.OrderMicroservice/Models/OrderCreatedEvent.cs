namespace csharp_choreography_saga.OrderMicroservice.Models
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail
    {
        public Guid ProductId { get; set; }
        public Guid InvoiceNo { get; set; }
        public long TotalItems { get; set; }
        public double SubTotal { get; set; }
    }
}
