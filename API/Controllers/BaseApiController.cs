using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController:ControllerBase
    {
        private IMediator mediator;
        protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected IActionResult HandleResult<T>(Result<T> result){
            if(result is null) return NotFound();
            if(result.IsSuccess && result.Value is not null){
                return Ok(result.Value);
            }
            if(result.IsSuccess && result.Value is null || result.Error.Contains("not found")){
                return NotFound();
            }
            return BadRequest(result.Error);
        }

    }
}