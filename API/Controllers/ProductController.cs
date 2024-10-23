using Application.DTOs;
using Application.Handler.Products;
using Application.SpecParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductController : BaseApiController
    {
        public ProductController()
        {
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] BaseSpecWithFilterParam baseSpecWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { BaseSpecWithFilterParam = baseSpecWithFilterParam }, ct));

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductInputDTO product, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { product = product }, ct));
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductInputDTO updatedProduct, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Edit.Command() { product = updatedProduct, Id = id }, ct));
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }
    }
}
