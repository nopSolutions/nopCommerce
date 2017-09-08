using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Tax;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Represents a shopping cart item extensions
    /// </summary>
    public static class ShoppingCartItemExtensions
    {
        /// <summary>
        /// Whether the shopping cart item is ship enabled
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="productService">Product service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <returns>True if the shopping cart item requires shipping; otherwise false</returns>
        public static bool IsShipEnabled(this ShoppingCartItem shoppingCartItem,
            IProductService productService = null, IProductAttributeParser productAttributeParser = null)
        {
            //whether the product requires shipping
            if (shoppingCartItem.Product != null && shoppingCartItem.Product.IsShipEnabled)
                return true;

            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return false;

            productService = productService ?? EngineContext.Current.Resolve<IProductService>();
            productAttributeParser = productAttributeParser ?? EngineContext.Current.Resolve<IProductAttributeParser>();

            //or whether associated products of the shopping cart item require shipping
            return productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .Any(attributeValue => productService.GetProductById(attributeValue.AssociatedProductId)?.IsShipEnabled ?? false);
        }

        /// <summary>
        /// Whether the shopping cart item is tax exempt
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="taxService">Tax service</param>
        /// <returns>True if the shopping cart item is tax exempt; otherwise false</returns>
        public static bool IsTaxExempt(this ShoppingCartItem shoppingCartItem, ITaxService taxService = null)
        {
            taxService = taxService ?? EngineContext.Current.Resolve<ITaxService>();

            return taxService.IsTaxExempt(shoppingCartItem.Product, shoppingCartItem.Customer);
        }
    }
}