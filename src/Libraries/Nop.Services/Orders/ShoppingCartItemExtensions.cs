using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
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
            IProductService productService, IProductAttributeParser productAttributeParser)
        {
            //whether the product requires shipping
            if (shoppingCartItem.Product != null && shoppingCartItem.Product.IsShipEnabled)
                return true;

            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return false;

            //or whether associated products of the shopping cart item require shipping
            return productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .Any(attributeValue => productService.GetProductById(attributeValue.AssociatedProductId)?.IsShipEnabled ?? false);
        }

        /// <summary>
        /// Whether the shopping cart item is free shipping
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="productService">Product service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <returns>True if the shopping cart item is free shipping; otherwise false</returns>
        public static bool IsFreeShipping(this ShoppingCartItem shoppingCartItem,
            IProductService productService, IProductAttributeParser productAttributeParser)
        {
            //first, check whether shipping is required
            if (!shoppingCartItem.IsShipEnabled(productService, productAttributeParser))
                return true;

            //then whether the product is free shipping
            if (shoppingCartItem.Product != null && !shoppingCartItem.Product.IsFreeShipping)
                return false;

            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return true;

            //and whether associated products of the shopping cart item is free shipping
            return productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .All(attributeValue => productService.GetProductById(attributeValue.AssociatedProductId)?.IsFreeShipping ?? true);
        }

        /// <summary>
        /// Get the additional shipping charge
        /// </summary> 
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="productService">Product service</param>
        /// <param name="productAttributeParser">Product attribute parser</param>
        /// <returns>The additional shipping charge of the shopping cart item</returns>
        public static decimal GetAdditionalShippingCharge(this ShoppingCartItem shoppingCartItem,
            IProductService productService, IProductAttributeParser productAttributeParser)
        {
            //first, check whether shipping is free
            if (shoppingCartItem.IsFreeShipping(productService, productAttributeParser))
                return decimal.Zero;

            //get additional shipping charge of the product
            var additionalShippingCharge = (shoppingCartItem.Product?.AdditionalShippingCharge ?? decimal.Zero) * shoppingCartItem.Quantity;

            if (string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
                return additionalShippingCharge;

            //and sum with associated products additional shipping charges
            additionalShippingCharge += productAttributeParser.ParseProductAttributeValues(shoppingCartItem.AttributesXml)
                .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                .Sum(attributeValue => productService.GetProductById(attributeValue.AssociatedProductId)?.AdditionalShippingCharge ?? decimal.Zero);

            return additionalShippingCharge;
        }

        /// <summary>
        /// Whether the shopping cart item is tax exempt
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="taxService">Tax service</param>
        /// <returns>True if the shopping cart item is tax exempt; otherwise false</returns>
        public static bool IsTaxExempt(this ShoppingCartItem shoppingCartItem, ITaxService taxService)
        {
            return taxService.IsTaxExempt(shoppingCartItem.Product, shoppingCartItem.Customer);
        }
    }
}