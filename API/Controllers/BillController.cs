using Infrastructure.Bill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class BillController : BaseApiController
    {
        [HttpGet("billbooking/{id}")]
        public async Task<IActionResult> CreateBillBooking(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new CreateBillAll.Query() { bookingId = id }, ct));
        }

        [HttpGet("billorder/{id}")]
        public async Task<IActionResult> CreateBillOrder(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new CreateBillOrder.Query() { orderId = id }, ct));
        }

    }
}