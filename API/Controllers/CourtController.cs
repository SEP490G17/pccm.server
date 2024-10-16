using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Handler.Courts;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CourtController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCourts(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(), ct));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourt(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateCourt([FromBody] Court court, CancellationToken ct)
        {
            court.Status = Domain.Enum.CourtStatus.Available;
            return HandleResult(await Mediator.Send(new Create.Command() { Court = court }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourt(int id, Court newCourt)
        {
            newCourt.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { court = newCourt }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
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
    }
}