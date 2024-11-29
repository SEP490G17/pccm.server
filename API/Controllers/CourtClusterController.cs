using Application.DTOs;
using Application.Handler.CourtClusters;
using Application.Handler.CourtClusters.UserSite;
using Application.SpecParams;
using Application.SpecParams.CourtClusterSpecification;
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

        [AllowAnonymous]
        [HttpGet("list-all-usersite")]
        public async Task<IActionResult> GetAllCourtClustersUserSite([FromQuery] CourtClusterSpecParam courtClusterSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListAllUserSite.Query() { CourtClusterSpecParam = courtClusterSpecParam }, ct));
        }

        /// <summary>
        ///  Trả về thông tin chi tiết của 1 cụm sân
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns> 
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourtCluster(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }
        /// <summary>
        /// Trả về thông tin chi tiết của 1 cụm sân phía user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns> 

        [HttpGet("usersite/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourtClusterUserSite(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new DetailUserSite.Query() { Id = id }, ct));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Owner, ManagerCourtCluster")]
        public async Task<IActionResult> CreateCourtCluster([FromBody] CourtClustersInputDto courtCluster, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { CourtCluster = courtCluster }, ct));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> UpdateCourtCluster(int id, CourtClustersInputDto newCourtCluster)
        {

            return HandleResult(await Mediator.Send(new Edit.Command() { courtCluster = newCourtCluster, id = id }));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> DeleteCourtCluster(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }

        [HttpPut("visible/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerCourtCluster")]
        public async Task<IActionResult> ToggleVisible([FromRoute] int id, [FromQuery] bool isVisible)
        {
            return HandleResult(await Mediator.Send(new ToggleVisible.Command() { Id = id, IsVisible = isVisible }));
        }

        [HttpGet("top-courtcluster")]
        [AllowAnonymous]
        public async Task<IActionResult> TopCourtCluster([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new TopCourtUserSite.Query(){ BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));
        }

    }
}