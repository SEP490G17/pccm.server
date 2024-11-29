using Application.Handler.Notifications;
using Application.SpecParams;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NotificationController : BaseApiController
    {

        [HttpGet]
        public async Task<IActionResult> GetUserNotification([FromQuery]BaseSpecParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetUserNoti.Query() { BaseSpecParam = baseSpecParam },ct));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReadNotification([FromRoute] int id){
            return HandleResult(await Mediator.Send(new UpdateReadAtNotification.Command() { NotiId = id }));
        }

    }
}