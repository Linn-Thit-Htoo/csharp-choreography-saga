using csharp_choreography_saga.OrderMicroservice.Entities;
using csharp_choreography_saga.OrderMicroservice.Models;
using csharp_choreography_saga.OrderMicroservice.Persistence.Base;
using Microsoft.EntityFrameworkCore;

namespace csharp_choreography_saga.OrderMicroservice.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryBase<TblOrder> _orderRepository;
        private readonly IRepositoryBase<TblOrderDetail> _orderDetailRepository;

        public OrderService(IRepositoryBase<TblOrder> orderRepository, IRepositoryBase<TblOrderDetail> orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task CompensateOrderAsync(CompensateOrderEvent compensateOrderEvent)
        {
            var order = await _orderRepository.GetByCondition(x => x.OrderId == compensateOrderEvent.OrderId)
                .FirstOrDefaultAsync();
            var orderLst = await _orderDetailRepository.GetByCondition(x => x.OrderId == compensateOrderEvent.OrderId)
                .ToListAsync();

            if (order is not null && orderLst is not null && orderLst.Count > 0)
            {
                _orderRepository.Delete(order);
                _orderDetailRepository.Delete(orderLst);

                await _orderRepository.SaveChangesAsync();
                await _orderDetailRepository.SaveChangesAsync();
            }
        }
    }
}
