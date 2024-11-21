using Application.DTOs;
using Application.Handler.CourtPrices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CourtPriceController : BaseApiController
    {
        [HttpPut("{id}/update")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> UpdatePriceOfCourt([FromRoute] int id
        , [FromBody] List<CourtPriceResponseDto> courtPrices, CancellationToken ct)
        {

            return HandleResult(await Mediator.Send(new UpdatePricesOfCourt.Command()
            {
                CourtId = id,
                CourtPriceResponseDtos = courtPrices,
            }, ct));
        }
    }
}