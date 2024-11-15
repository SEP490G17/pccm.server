using Application.Handler.Categories;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class CategoryController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCategories(CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query(), ct));
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostCategories([FromBody] Category category, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { Category = category }, ct));
        }
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category updatedCategory)
        {
            updatedCategory.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Category = updatedCategory }));
        }
    }
}