using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Handler.Orders;
using Domain.Entity;
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDTO orderInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { order = orderInput }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderInputDTO orderInput)
        {
            orderInput.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { order = orderInput }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}