using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Common;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/metafields")]
    public class MetafieldsController : ControllerBase
    {
        private readonly IGenericAttributeService _genericAttributeService;

        public MetafieldsController(IGenericAttributeService genericAttributeService)
        {
            _genericAttributeService = genericAttributeService;
        }

        /// <summary>
        /// Returns the generic attributes stored against one entity
        /// </summary>
        /// <remarks>
        /// Only reads are exposed. A write endpoint here would let a caller attach an arbitrary
        /// attribute to any entity type, which is a much broader capability than the rest of the
        /// write surface of this API and is left out on purpose.
        /// </remarks>
        // GET api/rest/metafields?entityType=product&entityId=1[&key=]
        [HttpGet]
        public async Task<ActionResult<List<MetafieldDto>>> GetForEntity(
            [FromQuery] string entityType,
            [FromQuery] int entityId,
            [FromQuery] string? key = null)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                return BadRequest(new { error = "An 'entityType' query value is required." });

            if (entityId <= 0)
                return BadRequest(new { error = "An 'entityId' query value greater than zero is required." });

            var attributes = await _genericAttributeService
                .GetAttributesForEntityAsync(entityId, ResolveKeyGroup(entityType));

            var result = attributes
                .Where(a => string.IsNullOrWhiteSpace(key) || string.Equals(a.Key, key.Trim(), StringComparison.OrdinalIgnoreCase))
                .Select(a => a.ToDto())
                .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Maps a friendly entity name onto the key group nopCommerce stores the attribute under
        /// </summary>
        /// <param name="entityType">Entity name, for example "product"</param>
        /// <returns>The key group to query with</returns>
        /// <remarks>
        /// nopCommerce has no constant for these, so the key groups are literals such as "Product" and
        /// "Customer" spread across the services. Anything not recognized is passed through unchanged, so
        /// the compound groups a plugin defines remain reachable.
        /// </remarks>
        protected virtual string ResolveKeyGroup(string entityType)
        {
            var trimmed = entityType.Trim();

            return trimmed.ToLowerInvariant() switch
            {
                "product" => "Product",
                "customer" => "Customer",
                "category" => "Category",
                "manufacturer" => "Manufacturer",
                "order" => "Order",
                "address" => "Address",
                "vendor" => "Vendor",
                _ => trimmed
            };
        }
    }
}
