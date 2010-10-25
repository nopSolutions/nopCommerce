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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Shipping manager interface
    /// </summary>
    public partial interface IShippingManager
    {
        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shopping cart weight</returns>
        decimal GetShoppingCartTotalWeight(ShoppingCart cart, Customer customer);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart, ref string error);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart, Customer customer);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, ref string error);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax, ref string error);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax, out decimal taxRate, ref string error);

        /// <summary>
        /// Gets shopping cart shipping total
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="taxRate">Applied tax rate</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping total</returns>
        decimal? GetShoppingCartShippingTotal(ShoppingCart cart,
            Customer customer, bool includingTax, out decimal taxRate,
            out Discount appliedDiscount, ref string error);

        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Shipping discount</returns>
        decimal GetShippingDiscount(Customer customer,
            decimal shippingTotal, out Discount appliedDiscount);
        
        /// <summary>
        /// Indicates whether the shopping cart requires shipping
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>True if the shopping cart requires shipping; otherwise, false.</returns>
        bool ShoppingCartRequiresShipping(ShoppingCart cart);

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="customer">Customer</param>
        /// <returns>Additional shipping charge</returns>
        decimal GetShoppingCartAdditionalShippingCharge(ShoppingCart cart, Customer customer);

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping options</returns>
        List<ShippingOption> GetShippingOptions(ShoppingCart cart,
            Customer customer, Address shippingAddress, ref string error);

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="allowedShippingRateComputationMethodId">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping options</returns>
        List<ShippingOption> GetShippingOptions(ShoppingCart cart,
            Customer customer, Address shippingAddress,
            int? allowedShippingRateComputationMethodId, ref string error);
    
        /// <summary>
        /// Gets or sets a default shipping origin address
        /// </summary>
        Address ShippingOrigin { get; set; }
    }
}
