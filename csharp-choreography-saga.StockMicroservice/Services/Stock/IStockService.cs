using csharp_choreography_saga.StockMicroservice.Models;

namespace csharp_choreography_saga.StockMicroservice.Services.Stock
{
    public interface IStockService
    {
        Task<bool> ReduceStockAsync(OrderCreatedEvent orderCreatedEvent);
    }
}
