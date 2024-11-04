using Application.DTOs;
using Application.Handler.Banners;
using Application.SpecParams;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BannerController : BaseApiController
    {
        public BannerController() { }

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
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBanner([FromQuery] BaseSpecParam baseSpecParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecParam = baseSpecParam }, ct));
        }

        [AllowAnonymous]
        [HttpGet("usersite")]
        public async Task<IActionResult> GetBannerUserSite(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListUserSite.Query(), ct));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetBanner(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Details.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostBanner([FromBody] BannerInputDto banner, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Banner = banner }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBanner(int id, BannerInputDto updatedBanner)
        {
            updatedBanner.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Banner = updatedBanner }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> ChangeStatus(int id, int status)
        {
            return HandleResult(await Mediator.Send(new ChangeStatus.Command() { Id = id, status = status }));
        }
    }
}
