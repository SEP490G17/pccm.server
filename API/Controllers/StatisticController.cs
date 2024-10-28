using Application.Handler.Statistics;
using Application.SpecParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class StatisticController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet("income")]
        public async Task<IActionResult> GetIncome(string? year, string? month, string? courtclusterId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Income.Query() { Year = year, Month = month, CourtClusterId = courtclusterId }, ct));
        }
    }
}
