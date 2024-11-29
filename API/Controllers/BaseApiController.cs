using API.SocketSignalR;
using Application.Core;
using Application.DTOs;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result is null) return NotFound();
            if (result.IsSuccess && result.Value is not null)
            {
                return Ok(result.Value);
            }
            if (result.IsSuccess && result.Value is null || result.Error.Contains("not found"))
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        protected async Task HandleAdminUpdateBookingRealTime(Result<BookingDtoV2> result)
        {
            if (result != null && result.IsSuccess)
            {
                var notificationUpdate = HttpContext.RequestServices.GetRequiredService<BookingRealTimeService>();
                await notificationUpdate.NotifyUpdateBookingAsync(result.Value, $"admin{result.Value.CourtClusterId}");
            }
        }

        protected async Task HandleUserUpdateBookingRealTime(Result<BookingDtoV2> result)
        {
            if (result != null && result.IsSuccess)
            {
                var notificationUpdate = HttpContext.RequestServices.GetRequiredService<BookingRealTimeService>();
                await notificationUpdate.NotifyUpdateBookingAsync(result.Value, $"user{result.Value.CourtClusterId}");
            }
        }

        protected async Task HandleAdminCreateBookingRealTime(Result<BookingDtoV1> result)
        {
            if (result != null && result.IsSuccess)
            {
                var notificationUpdate = HttpContext.RequestServices.GetRequiredService<BookingRealTimeService>();
                await notificationUpdate.NotifyCreateBookingAsync(result.Value, $"admin{result.Value.CourtClusterId}");
            }
        }


        protected async Task HandlUserCreateBookingRealTime(Result<BookingDtoV1> result)
        {
            if (result != null && result.IsSuccess && result.Value.Status == (int)BookingStatus.Confirmed)
            {
                var notificationUpdate = HttpContext.RequestServices.GetRequiredService<BookingRealTimeService>();
                await notificationUpdate.NotifyCreateBookingAsync(result.Value, $"user{result.Value.CourtClusterId}");
            }
        }

 

    }
}