
using Application.DTOs;
using Application.Handler.Bill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class BillController : BaseApiController
    {
        [HttpPost("billbooking")]
        public async Task<IActionResult> CreateBillBooking([FromBody] BillInputDto billInputDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new CreateBillAll.Query() { bookingId = billInputDto.bookingId, orderId = billInputDto.orderId }, ct));
        }

        [HttpGet("billorder/{id}")]
        public async Task<IActionResult> CreateBillOrder(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new CreateBillOrder.Query() { orderId = id }, ct));
        }

    }
}