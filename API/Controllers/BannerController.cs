using Application.DTOs;
using Application.Handler.Banners;
using Application.Interfaces;
using Application.SpecParams.ProductSpecification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BannerController(IUserAccessor userAccessor, ILogger<BannerController> logger) : BaseApiController
    {
        /*
            Không hiểu code đọc theo thứ tự:
            1. BaseSpecParam
            2. List
            3. ISpecification
            4. BaseSpecification
            5. GenericRepository
            6. SpecificationEvaluator
            7. UnitOfWork
        */

        [HttpGet]
        [Authorize(Roles = "Admin, Owner, ManagerBanner")]
        public async Task<IActionResult> GetBanner([FromQuery] BannerSpecParams baseSpecParam, CancellationToken ct)
        {
            var username = userAccessor.GetUserName();
            logger.LogInformation($"Check >>> {username}");

            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecParam = baseSpecParam }, ct));
        }

        [AllowAnonymous]
        [HttpGet("usersite")]
        public async Task<IActionResult> GetBannerUserSite(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListUserSite.Query(), ct));
        }

        [Authorize(Roles = "Admin, Owner, ManagerBanner")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBanner(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Details.Query() { Id = id }, ct));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Owner, ManagerBanner")]
        public async Task<IActionResult> PostBanner([FromBody] BannerInputDto banner, CancellationToken ct)
        {
            
            return HandleResult(await Mediator.Send(new Create.Command() { Banner = banner}, ct));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Owner, ManagerBanner")]
        public async Task<IActionResult> UpdateBanner(int id, BannerInputDto updatedBanner)
        {
            string userName = userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            updatedBanner.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Banner = updatedBanner, userName = userName }));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Owner, ManagerBanner")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            string userName = userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id, userName = userName }));
        }

        [HttpPut]
        [Authorize(Roles = "Admin, Owner, ManagerBanner")]
        public async Task<IActionResult> ChangeStatus(int id, int status)
        {
            return HandleResult(await Mediator.Send(new ChangeStatus.Command() { Id = id, status = status }));
        }
    }
}
