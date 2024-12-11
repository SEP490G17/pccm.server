using Application.DTOs;
using Application.Handler.Services;
using Application.SpecParams;
using Application.SpecParams.ProductSpecification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ServiceController : BaseApiController
    {

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetServices([FromQuery] BaseSpecWithFilterParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecParam = baseSpecParam }, ct));
        }
        [HttpGet("admin")]
        public async Task<IActionResult> GetServicesAdmin([FromQuery] BaseSpecWithFilterParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListAdmin.Query() { BaseSpecParam = baseSpecParam }, ct));
        }

        [HttpGet("log")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> GetServiceLog([FromQuery] ServiceLogSpecParams baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListServiceLog.Query() { BaseSpecParam = baseSpecParam }, ct));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetService(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> PostService([FromBody] ServiceInputDto service, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Service = service }, ct));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> UpdateService(int id, ServiceInputDto updatedService)
        {
           
            updatedService.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Service = updatedService }));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> DeleteService(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id}));
        }
    }
}
