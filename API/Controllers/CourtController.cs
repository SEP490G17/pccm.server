using Application.Handler.CourtPrices;
using Application.Handler.Courts;
using Application.SpecParams;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CourtController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCourts([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourt(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }


        /// <summary>
        /// Get Court list of a cluster 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("cluster/{id}")]
        public async Task<IActionResult> GetCourtOfCluster([FromRoute(Name = "id")] int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetListCourtOfCluster.Query() { CourtClusterId = id }, ct));
        }

        [HttpGet("{id}/prices")]
        public async Task<IActionResult> GetPriceOfCourt([FromRoute(Name = "id")] int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new GetCourtPricesOfCourt.Query() { CourtId = id }, ct));
        }

        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourtsList([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListByCourtCluster.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> CreateCourt([FromBody] Court court, CancellationToken ct)
        {
            court.Status = Domain.Enum.CourtStatus.Available;
            return HandleResult(await Mediator.Send(new Create.Command() { Court = court }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> UpdateCourt(int id, Court newCourt)
        {
            newCourt.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { court = newCourt }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> DeleteCourt(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }

        [AllowAnonymous]
        [HttpGet("search/{value}")]
        public async Task<IActionResult> SearchCourt(string value, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Search.Query() { value = value }, ct));
        }

        [AllowAnonymous]
        [HttpGet("filter/{valueFilter}")]
        public async Task<IActionResult> FilterCourt(string valueFilter, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Filter.Query() { value = valueFilter }, ct));
        }

        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> ToggleCourt([FromRoute] int id, [FromQuery] int status, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ToggleCourt.Command() { Id = id, Status = status }, ct));
        }
    }
}