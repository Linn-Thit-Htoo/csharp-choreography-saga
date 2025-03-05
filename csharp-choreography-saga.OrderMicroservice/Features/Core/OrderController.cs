using csharp_choreography_saga.OrderMicroservice.Features.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace csharp_choreography_saga.OrderMicroservice.Features.Core
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;

        public OrderController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrderAsync(CreateOrderCommand command, CancellationToken cs)
        {
            var result = await _sender.Send(command, cs);
            return Ok(result);
        }
    }
}
