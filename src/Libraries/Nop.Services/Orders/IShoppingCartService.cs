using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial interface IShoppingCartService
    {
        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false);

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        int DeleteExpiredShoppingCartItems(DateTime olderThanUtc);

        /// <summary>
        /// Validates required product variants (product variants which require other variant to be added to the cart)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="automaticallyAddRequiredProductVariantsIfEnabled">Automatically add required product variants if enabled</param>
        /// <returns>Warnings</returns>
        IList<string> GetRequiredProductVariantWarnings(Customer customer,
            ShoppingCartType shoppingCartType, ProductVariant productVariant,
            bool automaticallyAddRequiredProductVariantsIfEnabled);

        /// <summary>
        /// Validates a product variant for standard properties
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        IList<string> GetStandardWarnings(Customer customer, ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes,
            decimal customerEnteredPrice, int quantity);

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartItemAttributeWarnings(ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes);
        
        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes);
        
        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductVariantsIfEnabled">Automatically add required product variants if enabled</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a product variant for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredProductVariantWarnings">A value indicating whether we should validate required product variants (product variants which require other variant to be added to the cart)</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartItemWarnings(Customer customer, ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes, decimal customerEnteredPrice,
            int quantity, bool automaticallyAddRequiredProductVariantsIfEnabled,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredProductVariantWarnings = true);

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartWarnings(IList<ShoppingCartItem> shoppingCart,
            string checkoutAttributes, bool validateCheckoutAttributes);

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <returns>Found shopping cart item</returns>
        ShoppingCartItem FindShoppingCartItemInTheCart(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            ProductVariant productVariant,
            string selectedAttributes = "",
            decimal customerEnteredPrice = decimal.Zero);


        /// <summary>
        /// Add a product variant to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductVariantsIfEnabled">Automatically add required product variants if enabled</param>
        /// <returns>Warnings</returns>
        IList<string> AddToCart(Customer customer, ProductVariant productVariant,
            ShoppingCartType shoppingCartType, string selectedAttributes,
            decimal customerEnteredPrice, int quantity, bool automaticallyAddRequiredProductVariantsIfEnabled);

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        IList<string> UpdateShoppingCartItem(Customer customer, int shoppingCartItemId,
            int newQuantity, bool resetCheckoutData);
        
        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        void MigrateShoppingCart(Customer fromCustomer, Customer toCustomer);
    }
}
