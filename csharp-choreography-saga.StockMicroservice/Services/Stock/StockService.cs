using csharp_choreography_saga.StockMicroservice.Entities;
using csharp_choreography_saga.StockMicroservice.Models;
using csharp_choreography_saga.StockMicroservice.Persistence.Base;
using Microsoft.EntityFrameworkCore;

namespace csharp_choreography_saga.StockMicroservice.Services.Stock;

public class StockService : IStockService
{
    private readonly IRepositoryBase<TblStock> _stockRepository;
    private readonly ILogger<StockService> _logger;

    public StockService(IRepositoryBase<TblStock> stockRepository, ILogger<StockService> logger)
    {
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task<bool> ReduceStockAsync(OrderCreatedEvent orderCreatedEvent)
    {
        try
        {
            foreach (var item in orderCreatedEvent.OrderDetails)
            {
                var stock = await _stockRepository
                    .GetByCondition(x => x.ProductId == item.ProductId)
                    .SingleOrDefaultAsync();
                if (stock is null)
                {
                    _logger.LogError($"Stock Not Found.");
                    return false;
                }

                if (item.TotalItems > stock.Stock)
                {
                    _logger.LogError($"Insufficient Stock.");
                    return false;
                }

                var resultStock = stock.Stock - item.TotalItems;
                _stockRepository.Update(stock);
                await _stockRepository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Stock Reduction Error: {ex.ToString()}");
            return false;
        }
    }
}
