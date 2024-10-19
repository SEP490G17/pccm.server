using Application.DTOs;
using Application.Handler.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserController : BaseApiController
    {
        public UserController() { }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetUser(string id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IsActive(ActiveDTO activeDTO, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ActiveUser.Command() { user = activeDTO }, ct));
        }
    }
}
