using Application.DTOs;
using Application.Handler.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [AllowAnonymous]
        [HttpGet("Years")]
        public async Task<IActionResult> GetYear(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetYears.Query(), ct));
        }

        [AllowAnonymous]
        [HttpGet("ClusterStatistics")]
        public async Task<IActionResult> GetClusterStatisticsByDate([FromQuery] DateTime date, [FromQuery] int clusterId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StatisticCluster.Query() { Date = date, CourtClusterId = clusterId }, ct));
        }
        [AllowAnonymous]
        [HttpGet("ExpendStatistics")]
        public async Task<IActionResult> GetExpenditureStatistics([FromQuery] string month, [FromQuery] string year, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StatisticExpend.Query() { Month = month, Year = year }, ct));
        }
        [AllowAnonymous]
        [HttpGet("TopStatistics")]
        public async Task<IActionResult> GetTopStatistics([FromQuery] string month, [FromQuery] string year, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new TopStatisticsService.Query { Month = month, Year = year }, ct));
        }
    }
}
