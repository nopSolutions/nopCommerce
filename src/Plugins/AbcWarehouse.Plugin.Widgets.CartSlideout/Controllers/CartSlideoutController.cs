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

namespace AbcWarehouse.Plugin.Widgets.CartSlideout.Controllers
{
    public class CartSlideoutController : BaseController
    {
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;

        public CartSlideoutController(
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext)
        {
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShoppingCartItem([FromBody]UpdateShoppingCartItemModel model)
        {
            if (model == null || !model.IsValid())
            {
                return BadRequest();
            }

            var itemId = model.ShoppingCartItemId;
            var customer = await _workContext.GetCurrentCustomerAsync();

            // Get the item
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer);
            var shoppingCartItem = shoppingCart.FirstOrDefault(sci => sci.Id == itemId);
            if (shoppingCartItem == null)
            {
                return BadRequest($"Unable to find shopping cart item with id {itemId}");
            }

            // Manipulate the attributes
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(
                model.ProductAttributeMappingId);
            shoppingCartItem.AttributesXml = model.IsChecked.Value ?
                await ChangeProductAttributeAsync(
                    productAttributeMapping,
                    shoppingCartItem.AttributesXml,
                    model.ProductAttributeValueId) :
                _productAttributeParser.RemoveProductAttribute(
                    shoppingCartItem.AttributesXml,
                    productAttributeMapping);

            // Update the item
            await _shoppingCartService.UpdateShoppingCartItemAsync(
                    customer,
                    shoppingCartItem.Id,
                    shoppingCartItem.AttributesXml,
                    shoppingCartItem.CustomerEnteredPrice,
                    shoppingCartItem.RentalStartDateUtc,
                    shoppingCartItem.RentalEndDateUtc,
                    shoppingCartItem.Quantity);

            // conditional attributes - will need this for haulaway
            // var enabledAttributeMappingIds = new List<int>();
            // var disabledAttributeMappingIds = new List<int>();
            // var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            // foreach (var attribute in attributes)
            // {
            //     var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
            //     if (conditionMet.HasValue)
            //     {
            //         if (conditionMet.Value)
            //             enabledAttributeMappingIds.Add(attribute.Id);
            //         else
            //             disabledAttributeMappingIds.Add(attribute.Id);
            //     }
            // }

            return Json(new
            {
                SubtotalHtml = await RenderViewComponentToStringAsync("CartSlideoutSubtotal", new { sci = shoppingCartItem })
            });
        }

        // TODO:
        // Need to remove the existing mapped value if Delivery/Pickup
        // I think this will be the same with warranties and maybe pickup in store?
        private async Task<string> ChangeProductAttributeAsync(
            ProductAttributeMapping pam,
            string attributesXml,
            int productAttributeValueId)
        {
            var result = attributesXml;
            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
            // TODO: Turn "Warranty" into an AbcDeliveryConst
            if (productAttribute.Name == AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName ||
                productAttribute.Name == "Warranty")
            {
                result = _productAttributeParser.RemoveProductAttribute(
                    result,
                    pam);
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
