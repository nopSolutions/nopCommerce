using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbcWarehouse.Plugin.Widgets.CartSlideout.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Misc.AbcCore.Nop;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Mvc;
using Nop.Core.Domain.Orders;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Controllers
{
    public class CartSlideoutController : BaseController
    {
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IAbcProductAttributeService _productAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;

        public CartSlideoutController(
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IAbcProductAttributeService productAttributeService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext)
        {
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
        }

        // very similiar to OrderController.ProductDetails_AttributeChange
        [HttpPost]
        public async Task<IActionResult> Slideout_AttributeChange(int productId, IFormCollection form)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return new NullJsonResult();
            }

            var errors = new List<string>();
            var attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            var pams = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            foreach (var pam in pams)
            {
                // get the "pickup" delivery options pav (not the actual store)
                var pavs = await _productAttributeService.GetProductAttributeValuesAsync(pam.Id);

                var conditionMet = await _productAttributeParser.IsConditionMetAsync(pam, attributeXml);
                if (!conditionMet.HasValue)
                {
                    continue;
                }

                if (conditionMet.Value)
                {
                    enabledAttributeMappingIds.Add(pam.Id);
                }
                else
                {
                    disabledAttributeMappingIds.Add(pam.Id);
                }
            }

            var sci = new ShoppingCartItem()
            {
                CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id,
                ProductId = product.Id,
                AttributesXml = attributeXml
            };

            var isPickup = false;
            foreach (var element in form)
            {
                var pav = await _productAttributeService.GetProductAttributeValueByIdAsync(int.Parse(element.Value.ToString()));
                if (pav?.Name == AbcDeliveryConsts.PickupProductAttributeValueName)
                {
                    isPickup = true;
                    break;
                }
            }

            var isDeclineNewHoseSelected = false;
            foreach (var element in form)
            {
                var pav = await _productAttributeService.GetProductAttributeValueByIdAsync(int.Parse(element.Value.ToString()));
                if (pav?.Name == "Decline New Hose")
                {
                    isDeclineNewHoseSelected = true;
                    break;
                }
            }

            return Json(new
            {
                SubtotalHtml = await RenderViewComponentToStringAsync("CartSlideoutSubtotal", new { sci = sci }),
                EnabledAttributeMappingIds = enabledAttributeMappingIds.ToArray(),
                DisabledAttributeMappingIds = disabledAttributeMappingIds.ToArray(),
                IsPickup = isPickup,
                IsDeclineNewHoseSelected = isDeclineNewHoseSelected
            });
        }

        private async Task<string> ChangeProductAttributeAsync(
            ProductAttributeMapping pam,
            string attributesXml,
            int productAttributeValueId)
        {
            var result = attributesXml;
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
            // TODO: Turn "Warranty" into an AbcDeliveryConst
            // removes the existing option
            if (productAttribute.Name == AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName ||
                productAttribute.Name == "Warranty")
            {
                result = _productAttributeParser.RemoveProductAttribute(
                    result,
                    pam);
            }

            // If Delivery/Pickup, need to also clear the existing Pickup/Haulaway options
            if (productAttribute.Name == AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName)
            {
                var pickupPa = await _productAttributeService.GetProductAttributeByNameAsync("Pickup");
                var pickupPam = (await _productAttributeParser.ParseProductAttributeMappingsAsync(result)).FirstOrDefault(
                    pam => pam.ProductAttributeId == pickupPa.Id);
                if (pickupPam != null)
                {
                    result = _productAttributeParser.RemoveProductAttribute(
                        result,
                        pickupPam);
                }

                var haulAwayDeliveryPa = await _productAttributeService.GetProductAttributeByNameAsync(
                    AbcDeliveryConsts.HaulAwayDeliveryProductAttributeName);
                var haulAwayDeliveryPam = (await _productAttributeParser.ParseProductAttributeMappingsAsync(result)).FirstOrDefault(
                    pam => pam.ProductAttributeId == haulAwayDeliveryPa.Id);
                if (haulAwayDeliveryPam != null)
                {
                    result = _productAttributeParser.RemoveProductAttribute(
                        result,
                        haulAwayDeliveryPam);
                }

                var haulAwayDeliveryInstallPa = await _productAttributeService.GetProductAttributeByNameAsync(
                    AbcDeliveryConsts.HaulAwayDeliveryInstallProductAttributeName);
                var haulAwayDeliveryInstallPam = (await _productAttributeParser.ParseProductAttributeMappingsAsync(result)).FirstOrDefault(
                    pam => pam.ProductAttributeId == haulAwayDeliveryInstallPa.Id);
                if (haulAwayDeliveryInstallPam != null)
                {
                    result = _productAttributeParser.RemoveProductAttribute(
                        result,
                        haulAwayDeliveryInstallPam);
                }
            }

            // If 0 was passed, don't add another attribute, treated as only removal
            return productAttributeValueId != 0 ?
                _productAttributeParser.AddProductAttribute(
                    result,
                    pam,
                    productAttributeValueId.ToString()) :
                result;
        }
    }
}
