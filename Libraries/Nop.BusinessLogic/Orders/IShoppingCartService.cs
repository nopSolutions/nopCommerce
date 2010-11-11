//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Shopping cart service interface
    /// </summary>
    public partial interface IShoppingCartService
    {
        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThan">Older than date and time</param>
        void DeleteExpiredShoppingCartItems(DateTime olderThan);

        /// <summary>
        /// Deletes a shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">The shopping cart item identifier</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        void DeleteShoppingCartItem(int shoppingCartItemId, bool resetCheckoutData);

        /// <summary>
        /// Gets a shopping cart by customer session GUId
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="customerSessionGuid">The customer session identifier</param>
        /// <returns>Cart</returns>
        ShoppingCart GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum shoppingCartType,
            Guid customerSessionGuid);

        /// <summary>
        /// Gets a shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">The shopping cart item identifier</param>
        /// <returns>Shopping cart item</returns>
        ShoppingCartItem GetShoppingCartItemById(int shoppingCartItemId);

        /// <summary>
        /// Gets current user shopping cart
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <returns>Cart</returns>
        ShoppingCart GetCurrentShoppingCart(ShoppingCartTypeEnum shoppingCartType);

        /// <summary>
        /// Gets shopping cart
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <returns>Cart</returns>
        ShoppingCart GetCustomerShoppingCart(int customerId,
            ShoppingCartTypeEnum shoppingCartType);

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        decimal? GetShoppingCartTotal(ShoppingCart cart,
            int paymentMethodId, Customer customer);

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        decimal? GetShoppingCartTotal(ShoppingCart cart,
            int paymentMethodId, Customer customer, bool useRewardPoints);

        /// <summary>
        /// Gets shopping cart total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="appliedGiftCards">Applied gift cards</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <param name="redeemedRewardPoints">Reward points to redeem</param>
        /// <param name="redeemedRewardPointsAmount">Reward points amount in primary store currency to redeem</param>
        /// <returns>Shopping cart total;Null if shopping cart total couldn't be calculated now</returns>
        decimal? GetShoppingCartTotal(ShoppingCart cart,
            int paymentMethodId, Customer customer,
            out decimal discountAmount, out Discount appliedDiscount,
            out List<AppliedGiftCard> appliedGiftCards,
            bool useRewardPoints, out int redeemedRewardPoints, out decimal redeemedRewardPointsAmount);

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <returns>Error</returns>
        string GetShoppingCartSubTotal(ShoppingCart cart,
            Customer customer, out decimal discountAmount, out Discount appliedDiscount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount);

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <returns>Error</returns>
        string GetShoppingCartSubTotal(ShoppingCart cart,
            Customer customer, bool includingTax,
            out decimal discountAmount, out Discount appliedDiscount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount);

        /// <summary>
        /// Gets shopping cart subtotal
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="discountAmount">Applied discount amount</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="subTotalWithoutDiscount">Sub total (without discount)</param>
        /// <param name="subTotalWithDiscount">Sub total (with discount)</param>
        /// <param name="taxRates">Tax rates (of order sub total)</param>
        /// <returns>Error</returns>
        string GetShoppingCartSubTotal(ShoppingCart cart,
            Customer customer, bool includingTax,
            out decimal discountAmount, out Discount appliedDiscount,
            out decimal subTotalWithoutDiscount, out decimal subTotalWithDiscount,
            out SortedDictionary<decimal, decimal> taxRates);

        /// <summary>
        /// Gets an order discount (applied to order subtotal)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderSubTotal">Order subtotal</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Order discount</returns>
        decimal GetOrderSubtotalDiscount(Customer customer,
            decimal orderSubTotal, out Discount appliedDiscount);

        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Order discount</returns>
        decimal GetOrderTotalDiscount(Customer customer,
            decimal orderTotal, out Discount appliedDiscount);

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        List<string> GetShoppingCartItemWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes, decimal customerEnteredPrice,
            int quantity);

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        List<string> GetShoppingCartItemAttributeWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes, int quantity);

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="validateQuantity">Value indicating whether to validation quantity</param>
        /// <returns>Warnings</returns>
        List<string> GetShoppingCartItemAttributeWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes, int quantity, bool validateQuantity);

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <returns>Warnings</returns>
        List<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes);

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        List<string> GetShoppingCartWarnings(ShoppingCart shoppingCart, string checkoutAttributes, bool validateCheckoutAttributes);

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="cycleLength">Cycle length</param>
        /// <param name="cyclePeriod">Cycle period</param>
        /// <param name="totalCycles">Toital cycles</param>
        /// <returns>Error</returns>
        string GetReccuringCycleInfo(ShoppingCart shoppingCart,
            out int cycleLength, out int cyclePeriod, out int totalCycles);

        /// <summary>
        /// Add a product variant to shopping cart
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        List<string> AddToCart(ShoppingCartTypeEnum shoppingCartType,
            int productVariantId, string selectedAttributes,
            decimal customerEnteredPrice, int quantity);

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="newQuantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        List<string> UpdateCart(int shoppingCartItemId, int newQuantity,
            bool resetCheckoutData);
    }
}
