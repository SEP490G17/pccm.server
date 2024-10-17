using Application.Events;
using Application.Handler.News;
using Application.SpecParams;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NewsController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] BaseSpecParam baseSpecParam ,CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(){BaseSpecParam=baseSpecParam}, ct));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivity(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostCategories([FromBody] NewsBlog events, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Event = events }, ct));
        }
        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, NewsBlog updatedActivity)
        {
            updatedActivity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Event = updatedActivity }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}