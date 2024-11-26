using Application.DTOs;
using Application.Handler.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class OrderController : BaseApiController
    {

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrder(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }
        [HttpGet("v1/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderV1(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DetailV1.Query() { Id = id }, ct));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDto orderInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { order = orderInput }, ct));
        }

        [HttpPost("v1")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> CreateOrderV1([FromBody] OrderCreateV1.Command command, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(command, ct));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> UpdateOrder(int id, OrderInputDto orderInput)
        {
            orderInput.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { order = orderInput }));
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

        // // chạy swagger bị lỗi nên cmt lại
        [HttpPut("edit")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> UpdateOrderV1([FromBody] OrderEditV1.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster,ManagerBooking")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}