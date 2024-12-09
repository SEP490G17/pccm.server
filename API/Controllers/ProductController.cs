using Application.DTOs;
using Application.Handler.Products;
using Application.Interfaces;
using Application.SpecParams.ProductSpecification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductSpecParams specWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { SpecParam = specWithFilterParam }, ct));

        }
        [HttpGet("admin")]
        public async Task<IActionResult> GetProductsAdmin([FromQuery] ProductSpecParams specWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListAdmin.Query() { SpecParam = specWithFilterParam }, ct));

        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] Details.Query query, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(query, ct));

        }

        [HttpPost]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> PostProduct([FromBody] ProductInputDto product, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new Create.Command() { product = product }, ct));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, ProductInputDto updatedProduct, CancellationToken ct)
        {

            return HandleResult(await Mediator.Send(new Edit.Command() { product = updatedProduct, Id = id }, ct));
        }

        [HttpPut("import/{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> ImportProduct([FromRoute] int id, ProductImportDto productImport, CancellationToken ct)
        {
           
            return HandleResult(await Mediator.Send(new ImportProduct.Command() { product = productImport, Id = id }, ct));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id }));
        }

        [HttpGet("log")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> GetProductsLog([FromQuery] ProductLogSpecParams specWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListProductLog.Query() { SpecParam = specWithFilterParam }, ct));

        }
    }
}
