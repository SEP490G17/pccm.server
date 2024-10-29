using Application.DTOs;
using Application.Handler.Statistics;
using Application.SpecParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class StatisticController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetStatistic([FromQuery] StatisticInputDTO statisticInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StatisticService.Query() { StatisticInput = statisticInput }, ct));
        }

        [AllowAnonymous]
        [HttpGet("Count")]
        public async Task<IActionResult> GetStatisticCount(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Count.Query(), ct));
        }
    }
}
