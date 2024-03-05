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
using Nop.Plugin.Misc.AbcCore.Mattresses;

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Controllers
{
    public class CartSlideoutController : BaseController
    {
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IAbcMattressModelService _abcMattressModelService;
        private readonly IAbcProductAttributeService _productAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;

        public CartSlideoutController(
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IAbcMattressModelService abcMattressModelService,
            IAbcProductAttributeService productAttributeService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext)
        {
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _abcMattressModelService = abcMattressModelService;
            _productAttributeService = productAttributeService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
        }

        // very similiar to OrderController.ProductDetails_AttributeChange
        [HttpPost]
        public async Task<IActionResult> Slideout_AttributeChange(int productId, IFormCollection form)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
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
            bool isWarrantyRequired = false;
            foreach (var pam in pams)
            {
                var pa = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                if (pa?.Name == AbcDeliveryConsts.WarrantyProductAttributeName)
                {
                    isWarrantyRequired = true;
                }

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
                CustomerId = customer.Id,
                ProductId = product.Id,
                AttributesXml = attributeXml
            };

            var nopWarnings = await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(
                customer, ShoppingCartType.ShoppingCart, product, 1, attributeXml
            );

            var isPickup = await IsPickupAsync(form);
            var isMattress = IsMattress(productId);
            var isWarrantySelected = isWarrantyRequired ? await IsWarrantySelectedAsync(form) : false;

            var isAddEditCartAllowed = isMattress ||
                (!nopWarnings.Any() && 
                (!isWarrantyRequired || isWarrantySelected));

            return Json(new
            {
                EnabledAttributeMappingIds = enabledAttributeMappingIds.ToArray(),
                DisabledAttributeMappingIds = disabledAttributeMappingIds.ToArray(),
                IsPickup = isPickup,
                IsMattress = isMattress,
                IsAddEditCartAllowed = isAddEditCartAllowed
            });
        }

        private async Task<bool> IsWarrantySelectedAsync(IFormCollection form)
        {
            foreach (var element in form)
            {
                var pamId = int.Parse(element.Key.Split("_")[2]);
                var pam = await _productAttributeService.GetProductAttributeMappingByIdAsync(pamId);
                if (pam is null) { continue; }
                
                var pa = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                if (pa?.Name == AbcDeliveryConsts.WarrantyProductAttributeName)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsMattress(int productId)
        {
            var abcMattressModel = _abcMattressModelService.GetAbcMattressModelByProductId(productId);
            return abcMattressModel is not null;
        }

        private async Task<bool> IsPickupAsync(IFormCollection form)
        {
            foreach (var element in form)
            {
                var pav = await _productAttributeService.GetProductAttributeValueByIdAsync(
                    int.Parse(element.Value.ToString())
                );
                if (pav?.Name == AbcDeliveryConsts.PickupProductAttributeValueName)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<string> ChangeProductAttributeAsync(
            ProductAttributeMapping pam,
            string attributesXml,
            int productAttributeValueId)
        {
            var result = attributesXml;
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
            // ABCTODO: Turn "Warranty" into an AbcDeliveryConst
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
