using Application.DTOs;
using Application.Handler.CourtClusters;
using Application.SpecParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CourtClusterController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCourtClusters([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

        [AllowAnonymous]
        [HttpGet("list-all")]
        public async Task<IActionResult> GetAllCourtClusters(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListAll.Query(), ct));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourtCluster(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateCourtCluster([FromBody] CourtClustersInputDTO courtCluster, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { CourtCluster = courtCluster }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourtCluster(int id, CourtClustersInputDTO newCourtCluster)
        {
            newCourtCluster.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { courtCluster = newCourtCluster }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourtCluster(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}