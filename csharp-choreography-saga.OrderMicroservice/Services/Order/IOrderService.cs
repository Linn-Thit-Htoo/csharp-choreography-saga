using csharp_choreography_saga.OrderMicroservice.Models;

namespace csharp_choreography_saga.OrderMicroservice.Services.Order
{
    public interface IOrderService
    {
        Task CompensateOrderAsync(CompensateOrderEvent compensateOrderEvent);
    }
}
