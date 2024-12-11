using Application.Events;
using Application.Handler.News;
using Application.SpecParams;
using Application.SpecParams.NewsSpecification;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NewsController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] BaseSpecWithFilterParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecParam = baseSpecParam }, ct));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivity(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [HttpGet("usersite")]
        [AllowAnonymous]
        public async Task<IActionResult> ListNewsUserSite([FromQuery] NewsSpecParams newsSpecParams, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListNewsUserSite.Query() { NewsSpecParams = newsSpecParams }, ct));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner,ManagerNews")]
        public async Task<IActionResult> PostCategories([FromBody] NewsBlog events, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Event = events }, ct));
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerNews")]
        public async Task<IActionResult> UpdateActivity(int id, NewsBlog updatedActivity)
        {
            updatedActivity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Event = updatedActivity }));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerNews")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }

        [HttpPut("changestatus/{id}/{status}")]
        [Authorize(Roles = "Admin,Owner,ManagerNews")]
        public async Task<IActionResult> ChangeStatus(int id, int status)
        {
            return HandleResult(await Mediator.Send(new ChangeStatus.Command() { Id = id, status = status }));
        }

        [AllowAnonymous]
        [HttpGet("most-common-tags")]
        public async Task<IActionResult> GetMostCommonTags(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetMostCommonTags.Query(), ct));
        }
    }
}