using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Web.Framework.Controllers;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Controllers
{
    public class ApiController : BasePluginController
    {
        private readonly IProductAttributeService _productAttributeService;

        public ApiController(
            IProductAttributeService productAttributeService) {
            _productAttributeService = productAttributeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductAttributeValue(int productAttributeValueId)
        {
            var productAttributeValue = await _productAttributeService.GetProductAttributeValueByIdAsync(productAttributeValueId);
            if (productAttributeValue == null)
            {
                return BadRequest($"ProductAttributeValue with ID {productAttributeValueId} not found.");
            }

            return Json(productAttributeValue);
        }
    }
}
