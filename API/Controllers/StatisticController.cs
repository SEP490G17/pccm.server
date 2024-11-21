using Application.DTOs;
using Application.Handler.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class StatisticController : BaseApiController
    {
        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetStatistic([FromQuery] StatisticInputDTO statisticInput, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StatisticService.Query() { StatisticInput = statisticInput }, ct));
        }

        [HttpGet("Count")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetStatisticCount(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Count.Query(), ct));
        }

        [HttpGet("Years")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetYear(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetYears.Query(), ct));
        }

        [HttpGet("ClusterStatistics")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetClusterStatisticsByDate([FromQuery] DateTime date, [FromQuery] int clusterId, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StatisticCluster.Query() { Date = date, CourtClusterId = clusterId }, ct));
        }
        [HttpGet("ExpendStatistics")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetExpenditureStatistics([FromQuery] string month, [FromQuery] string year, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new StatisticExpend.Query() { Month = month, Year = year }, ct));
        }
        [HttpGet("TopStatistics")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetTopStatistics([FromQuery] string month, [FromQuery] string year, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new TopStatisticsService.Query { Month = month, Year = year }, ct));
        }

        [HttpGet("BookingRecently")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> BookingRecently(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new BookingRecently.Query(), ct));
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> AddExpense([FromBody] ExpenseDto expenseDto, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new CreateExpense.Command() { expenseDto = expenseDto }, ct));
        }

        [HttpPost("SaveRevenue")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> SaveRevenue([FromBody] SaveRevenueDto data, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new SaveRevenue.Command() { revenue = data }, ct));
        }

        [HttpGet("ExportExcel")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime date, [FromQuery] int clusterId, CancellationToken ct)
        {
            var result = await Mediator.Send(new ExportExcel.Query() { Date = date, CourtClusterId = clusterId }, ct);
            return HandleResult(result);
        }
    }
}
