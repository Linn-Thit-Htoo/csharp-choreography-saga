using csharp_choreography_saga.OrderMicroservice.Entities;
using csharp_choreography_saga.OrderMicroservice.Models;
using csharp_choreography_saga.OrderMicroservice.Persistence.Base;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace csharp_choreography_saga.OrderMicroservice.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext appDbContext, ILogger<OrderService> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task CompensateOrderAsync(CompensateOrderEvent compensateOrderEvent)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                var order = await _appDbContext.TblOrders
        .Include(x => x.TblOrderDetails)
        .SingleOrDefaultAsync(x => x.OrderId == compensateOrderEvent.OrderId)
        ?? throw new Exception("Order not found.");

                foreach (var item in order.TblOrderDetails)
                {
                    item.IsDeleted = true;
                    _appDbContext.TblOrderDetails.Update(item);
                }

                order.IsDeleted = true;
                _appDbContext.TblOrders.Update(order);

                await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Compensating Done!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
