using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Handler.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BookingController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBookings(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(), ct));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBooking(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}/{status}")]
        public async Task<IActionResult> UpdateStatusBooking(int id, string status)
        {
            return HandleResult(await Mediator.Send(new Edit.Command() { Id = id, Status = status }));
        }

    }
}