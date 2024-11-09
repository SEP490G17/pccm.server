using Application.DTOs;
using Application.Handler.Bookings;
using Application.SpecParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BookingController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

        // [AllowAnonymous]
        // [HttpGet("v2")]
        // public async Task<IActionResult> GetBookingsV2([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        // {
        //     return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        // }

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

        [AllowAnonymous]
        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] string date, [FromQuery] int courtClusterId)
        {

            return HandleResult(await Mediator.Send(new GetSlot.Query() { Date = date, CourtClusterId = courtClusterId }));
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingInputDto bookingInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Booking = bookingInput }, ct));
        }

    }
}