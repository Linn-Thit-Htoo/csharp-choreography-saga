namespace csharp_choreography_saga.OrderMicroservice.Models;

public class CompensateOrderEvent
{
    public Guid OrderId { get; set; }
}
