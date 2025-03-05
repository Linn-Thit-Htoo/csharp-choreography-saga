using csharp_choreography_saga.OrderMicroservice.Constants;
using csharp_choreography_saga.OrderMicroservice.Entities;
using csharp_choreography_saga.OrderMicroservice.Models;
using csharp_choreography_saga.OrderMicroservice.Persistence.Base;
using csharp_choreography_saga.OrderMicroservice.Services.RabbitMQ;
using csharp_choreography_saga.OrderMicroservice.Utils;
using MediatR;
using System.Transactions;

namespace csharp_choreography_saga.OrderMicroservice.Features.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
    {
        private readonly IRepositoryBase<TblOrder> _orderRepository;
        private readonly IRepositoryBase<TblOrderDetail> _orderDetailRepository;
        private readonly IBus _bus;

        public CreateOrderCommandHandler(IRepositoryBase<TblOrder> orderRepository, IRepositoryBase<TblOrderDetail> orderDetailRepository, IBus bus)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _bus = bus;
        }

        public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Result<CreateOrderResponse> result;
            var orderDetailLst = new List<TblOrderDetail>();
            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled
            );

            var order = new TblOrder()
            {
                UserId = request.UserId,
                InvoiceNo = Guid.NewGuid(),
                GrandTotal = request.GrandTotal,
                Status = nameof(OrderStatus.Pending)
            };
            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            if (request.OrderDetails is not null && request.OrderDetails.Count > 0)
            {
                foreach (var item in request.OrderDetails)
                {
                    orderDetailLst.Add(new TblOrderDetail()
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        InvoiceNo = order.InvoiceNo,
                        SubTotal = item.SubTotal,
                        TotalItems = item.TotalItems
                    });
                }
            }
            await _orderDetailRepository.AddAsync(orderDetailLst, cancellationToken);
            await _orderDetailRepository.SaveChangesAsync(cancellationToken);

            scope.Complete();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                OrderId = order.OrderId,
                OrderDetails = request.OrderDetails!.Select(x => new OrderDetail()
                {
                    ProductId = x.ProductId,
                    InvoiceNo = order.InvoiceNo,
                    SubTotal = x.SubTotal,
                    TotalItems = x.TotalItems
                }).ToList()
            };

            _bus.Send("DirectExchange", "StockQueue", "orderdirect", orderCreatedEvent);

            var model = new CreateOrderResponse() { OrderId = order.OrderId };
            result = Result<CreateOrderResponse>.Success(model);

            return result;
        }
    }
}
