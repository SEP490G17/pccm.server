using Application.DTOs;
using Application.Handler.CourtCombos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CourtComboController : BaseApiController
    {
        [HttpPut("{id}/update")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> UpdateCourtCombos([FromBody] List<CourtComboDto> courtCombos, [FromRoute] int id,
        CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new CreateCourtCombo.Command()
            {
                CourtId = id,
                CourtComboCreateDtos = courtCombos
            }, ct));
        }

    }
}