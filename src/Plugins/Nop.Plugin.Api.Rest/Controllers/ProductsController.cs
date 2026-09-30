using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Plugin.Api.Rest.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET api/rest/products?page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var products = await _productService.SearchProductsAsync(pageIndex: page - 1, pageSize: pageSize);
            var dto = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Price = p.Price
            });
            return Ok(dto);
        }

        // GET api/rest/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price
            };
            return Ok(dto);
        }
    }
}
