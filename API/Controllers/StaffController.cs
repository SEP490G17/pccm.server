using Application.Handler.Staffs;
using Application.SpecParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class StaffController : BaseApiController
    {
        [HttpGet]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]
        public async Task<IActionResult> GetStaffs([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerStaff")]

        public async Task<IActionResult> GetStaff(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

    }
}