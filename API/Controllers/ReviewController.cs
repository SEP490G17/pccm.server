using Application.DTOs;
using Application.Handler.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ReviewController : BaseApiController
    {
        public ReviewController()
        {
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetReview(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(), ct));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetReview(int id, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Detail.Query() { Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] ReviewInputDTO review, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { review = review }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, ReviewInputDTO updatedReview)
        {
            updatedReview.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { review = updatedReview }));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}
