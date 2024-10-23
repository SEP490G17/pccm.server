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
    }
}