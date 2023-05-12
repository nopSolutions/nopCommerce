using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;

namespace Nop.Plugin.CustomAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> _productsRepository;

        public ProductController(IRepository<Product> productrepository)
        {
            _productsRepository = productrepository;
        }


        [HttpGet(Name = "GetAllProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = _productsRepository.GetAll();
            return Ok(products);
        }

        [HttpGet("GetProductByName/{search}", Name = "GetProductByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProductByString(string? search)
        {
            var products = _productsRepository.GetAll();

            if (search != null)
            {
                var product1 = products.Where(x => x.Name.Contains(search) || x.Sku.Contains(search)).ToList();
                if (product1.Count == 0)
                {
                    return NotFound();
                }
                return Ok(product1);
            }

            return BadRequest();
        }

        [HttpGet("{id}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProductById([FromRoute] int id)
        {
            if (id > 0)
            {
                var products = _productsRepository.GetByIdAsync(id);
                if (products == null)
                {
                    return NotFound();
                }
                return Ok(products.Result);
            }

            return BadRequest();
        }
    }
}