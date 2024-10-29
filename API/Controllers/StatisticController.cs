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
        [HttpGet("income")]
        public async Task<IActionResult> GetIncome([FromQuery] StatisticInputDTO statisticInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Income.Query() { StatisticInput = statisticInput }, ct));
        }
    }
}
