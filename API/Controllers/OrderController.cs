using Application.DTOs;
using Application.Handler.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class OrderController : BaseApiController
    {

        [HttpGet("v1/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderV1(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DetailV1.Query() { Id = id }, ct));
        }

        [HttpPost("v1")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> CreateOrderV1([FromBody] OrderCreateV1.Command command, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(command, ct));
        }

        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> CancelOrder([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new CancelOrderV1.Command() { Id = id }));
        }

        [HttpPut("complete/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> CompleteOrder([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new CompleteOrder.Command() { Id = id }));
        }

        [HttpPut("edit")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> UpdateOrderV1([FromBody] OrderEditV1.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

    }
}