using csharp_choreography_saga.OrderMicroservice.Utils;
using MediatR;

namespace csharp_choreography_saga.OrderMicroservice.Features.CreateOrder;

public class CreateOrderCommand : IRequest<Result<CreateOrderResponse>>
{
    public Guid UserId { get; set; }

    public double GrandTotal { get; set; }

    public List<CreateOrderDetailCommand> OrderDetails { get; set; }
}

public class CreateOrderDetailCommand
{
    public Guid ProductId { get; set; }

    public long TotalItems { get; set; }

    public double SubTotal { get; set; }
}
