using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial interface IPriceCalculationService
    {
        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        decimal GetFinalPrice(Product product,
            Customer customer,
            decimal additionalCharge = decimal.Zero, 
            bool includeDiscounts = true, 
            int quantity = 1);

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="quantity">Product quantity</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        decimal GetDiscountAmount(Product product, 
            Customer customer,
            decimal additionalCharge, 
            int quantity, 
            out Discount appliedDiscount);
        



        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        decimal GetSubTotal(ShoppingCartItem shoppingCartItem, bool includeDiscounts);
        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        decimal GetUnitPrice(ShoppingCartItem shoppingCartItem, bool includeDiscounts);
        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Product atrributes (XML format)</param>
        /// <param name="customerEnteredPrice">Customer entered price (if specified)</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        decimal GetUnitPrice(Product product,
            Customer customer,
            ShoppingCartType shoppingCartType,
            int quantity,
            string attributesXml,
            decimal customerEnteredPrice,
            bool includeDiscounts);
        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        decimal GetDiscountAmount(ShoppingCartItem shoppingCartItem, out Discount appliedDiscount);




        /// <summary>
        /// Gets the product cost (one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Shopping cart item attributes in XML</param>
        /// <returns>Product cost (one item)</returns>
        decimal GetProductCost(Product product, string attributesXml);



        
        /// <summary>
        /// Get a price adjustment of a product variant attribute value
        /// </summary>
        /// <param name="pvav">Product variant attribute value</param>
        /// <returns>price adjustment</returns>
        decimal GetProductVariantAttributeValuePriceAdjustment(ProductVariantAttributeValue pvav);
    }
}
