using Application.DTOs;
using Application.Handler.CourtPrices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CourtPriceController : BaseApiController
    {
        [HttpPut("{id}/update")]
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