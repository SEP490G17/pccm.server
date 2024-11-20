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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDto orderInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { order = orderInput }, ct));
        }

        [AllowAnonymous]
        [HttpPost("v1")]
        public async Task<IActionResult> CreateOrderV1([FromBody] OrderCreateV1.Command command, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(command, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderInputDto orderInput)
        {
            orderInput.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { order = orderInput }));
        }

        [AllowAnonymous]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(int id, OrderInputDto orderInput)
        {
            orderInput.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { order = orderInput }));
        }


        // [AllowAnonymous]
        // [HttpPut("v1/edit")]
        // public async Task<IActionResult> UpdateOrderV1([FromBody] OrderEditV1.Command command)
        // {
        //     return HandleResult(await Mediator.Send(command));
        // }


        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}