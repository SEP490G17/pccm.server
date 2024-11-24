using Application.DTOs;
using Application.Handler.Services;
using Application.Interfaces;
using Application.SpecParams;
using Application.SpecParams.ProductSpecification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ServiceController : BaseApiController
    {
        private readonly IUserAccessor _userAccessor;
        public ServiceController(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetService([FromQuery] BaseSpecWithFilterParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecParam = baseSpecParam }, ct));
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
            string userName = _userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            return HandleResult(await Mediator.Send(new Create.Command() { Service = service, userName = userName }, ct));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> UpdateService(int id, ServiceInputDto updatedService)
        {
            string userName = _userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            updatedService.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Service = updatedService, userName = userName }));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> DeleteService(int id)
        {
            string userName = _userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id, userName = userName }));
        }
    }
}
