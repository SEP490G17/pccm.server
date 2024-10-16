using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategories(int id, Category category, CancellationToken ct)
        {
            category.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command() { Category = category }, ct));

        }

    }
}