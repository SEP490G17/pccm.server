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
        private readonly IUserAccessor _userAccessor;
        public ProductController(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductSpecParams specWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new List.Query() { SpecParam = specWithFilterParam }, ct));

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
            string userName = _userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            return HandleResult(await Mediator.Send(new Create.Command() { product = product, userName = userName }, ct));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, ProductInputDto updatedProduct, CancellationToken ct)
        {
            string userName = _userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            return HandleResult(await Mediator.Send(new Edit.Command() { product = updatedProduct, Id = id, userName = userName }, ct));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Owner,ManagerSupplies,ManagerCourtCluster")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            string userName = _userAccessor.GetUserName();
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(new { Message = "User is not authenticated" }); // Return a message with a 400 BadRequest status 
            }
            return HandleResult(await Mediator.Send(new Delete.Command() { Id = id, userName = userName }));
        }

        [HttpGet("log")]
        public async Task<IActionResult> GetProductsLog([FromQuery] ProductLogSpecParams specWithFilterParam, CancellationToken ct)
        {
            return HandleResult(await Mediator.Send(new ListProductLog.Query() { SpecParam = specWithFilterParam }, ct));

        }
    }
}
