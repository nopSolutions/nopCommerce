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
using System.Linq;
using Nop.Services.Localization;
using Nop.Services.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Directory;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial interface IShoppingCartService
    {
        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        void DeleteExpiredShoppingCartItems(DateTime olderThanUtc);

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="productVariant">Product variant</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        IList<string> GetShoppingCartItemWarnings(ShoppingCartType shoppingCartType,
            ProductVariant productVariant, string selectedAttributes, decimal customerEnteredPrice, 
            int quantity);

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
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="selectedAttributes">Selected attributes</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <returns>Found shopping cart item</returns>
        ShoppingCartItem FindShoppingCartItemInTheCart(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            int productVariantId,
            string selectedAttributes = "",
            decimal customerEnteredPrice = decimal.Zero);
    }
}
