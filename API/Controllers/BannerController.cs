using Application.Handler.Banners;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace API.Controllers
{
    public class BannerController : BaseApiController
    {
        public BannerController() { }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBanner(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(),ct));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetBanner(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Details.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostBanner([FromBody] Banner banner, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Banner = banner }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBanner(int id, Banner updatedBanner)
        {
            updatedBanner.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Banner = updatedBanner}));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}
