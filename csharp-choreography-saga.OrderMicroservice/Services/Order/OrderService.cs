using csharp_choreography_saga.OrderMicroservice.Configurations;
using csharp_choreography_saga.OrderMicroservice.Entities;
using csharp_choreography_saga.OrderMicroservice.Models;
using csharp_choreography_saga.OrderMicroservice.Persistence.Base;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace csharp_choreography_saga.OrderMicroservice.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<OrderService> _logger;
        private readonly IRepositoryBase<TblOrder> _orderRepository;
        private readonly AppSetting _appSetting;

        public OrderService(IOptions<AppSetting> options, AppDbContext appDbContext, ILogger<OrderService> logger, IRepositoryBase<TblOrder> orderRepository)
        {
            _appSetting = options.Value;
            _appDbContext = appDbContext;
            _logger = logger;
            _orderRepository = orderRepository;
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

        public async Task CompensateOrderAsyncV1(CompensateOrderEvent compensateOrderEvent)
        {
            try
            {
                string query = @"DELETE FROM ""Tbl_Order"" WHERE ""OrderId"" = @OrderId";
                var parameters = new
                {
                    compensateOrderEvent.OrderId
                };
                var db = new NpgsqlConnection(_appSetting.ConnectionStrings.DbConnection);
                int result = await db.ExecuteAsync(query, parameters);

                //var order = await _orderRepository
                //    .GetByCondition(x => x.OrderId == compensateOrderEvent.OrderId)
                //    .SingleOrDefaultAsync()
                //    ?? throw new Exception("Order not found.");

                //_orderRepository.Delete(order);
                //await _orderRepository.SaveChangesAsync();

                _logger.LogInformation("Compensating Done!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Rollback Order: {ex.ToString()}");
                throw;
            }
        }
    }
}
