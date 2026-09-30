using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/products")]
    public class ProductAttributesController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributesController(IProductService productService,
            IProductAttributeService productAttributeService)
        {
            _productService = productService;
            _productAttributeService = productAttributeService;
        }

        /// <summary>
        /// Returns the attributes mapped to a product together with the values that can be selected
        /// </summary>
        /// <remarks>
        /// Declared before the {id:int} route of the sibling controller would matter for MVC routing, so
        /// it lives under its own sub path to keep the two apart.
        /// </remarks>
        // GET api/rest/products/attributes/{productId}
        [HttpGet("attributes/{productId:int}")]
        public async Task<ActionResult<List<ProductAttributeDto>>> GetByProduct(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound();

            var mappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            var attributeIds = mappings.Select(m => m.ProductAttributeId).Distinct().ToArray();
            var attributes = await _productAttributeService.GetProductAttributeByIdsAsync(attributeIds);
            var attributesById = attributes.ToDictionary(a => a.Id);

            var result = new List<ProductAttributeDto>();
            foreach (var mapping in mappings.OrderBy(m => m.DisplayOrder))
            {
                //a mapping can outlive the attribute it points at, so skip the ones that no longer resolve
                if (!attributesById.TryGetValue(mapping.ProductAttributeId, out var attribute))
                    continue;

                var values = await _productAttributeService.GetProductAttributeValuesAsync(mapping.Id);

                result.Add(new ProductAttributeDto
                {
                    ProductAttributeId = attribute.Id,
                    MappingId = mapping.Id,
                    Name = attribute.Name,
                    TextPrompt = mapping.TextPrompt,
                    IsRequired = mapping.IsRequired,
                    AttributeControlTypeId = mapping.AttributeControlTypeId,
                    DisplayOrder = mapping.DisplayOrder,
                    Values = values.OrderBy(v => v.DisplayOrder).Select(v => v.ToDto()).ToList()
                });
            }

            return Ok(result);
        }
    }
}
