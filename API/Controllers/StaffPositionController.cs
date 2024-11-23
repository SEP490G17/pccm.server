using Application.DTOs;
using Application.Handler.StaffPositions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class StaffPositionController : BaseApiController
    {
        [HttpGet]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]
        public async Task<IActionResult> GetStaffPositions(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(), ct));
        }

        [HttpGet("applyToAll")]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]
        public async Task<IActionResult> applyRoleToAll(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ApplyToAll.Command(), ct));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]
        public async Task<IActionResult> UpdateStaffRoles([FromBody] List<StaffRoleInputDto> role, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new UpdateRole.Command() { data = role }, ct));
        }
    }
}