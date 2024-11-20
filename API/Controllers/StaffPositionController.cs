using Application.DTOs;
using Application.Handler.StaffPositions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class StaffPositionController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetStaffPositions(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(), ct));
        }

        [AllowAnonymous]
        [HttpGet("applyToAll")]
        public async Task<IActionResult> applyRoleToAll(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ApplyToAll.Command(), ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UpdateStaffRoles([FromBody] List<StaffRoleInputDto> role, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new UpdateRole.Command() { data = role }, ct));
        }
    }
}