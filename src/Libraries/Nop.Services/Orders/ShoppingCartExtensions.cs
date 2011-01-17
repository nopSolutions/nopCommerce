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

using System.Collections.Generic;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Represents a shopping cart
    /// </summary>
    public static class ShoppingCartExtensions
    {
        /// <summary>
        /// Gets a value indicating whether shopping cart is recurring
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>Result</returns>
        public static bool IsRecurring(this IList<ShoppingCartItem> shoppingCart)
        {
            foreach (ShoppingCartItem sci in shoppingCart)
            {
                var productVariant = sci.ProductVariant;
                if (productVariant != null)
                {
                    if (productVariant.IsRecurring)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a number of product in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>Result</returns>
        public static int GetTotalProducts(this IList<ShoppingCartItem> shoppingCart)
        {
            int result = 0;
            foreach (ShoppingCartItem sci in shoppingCart)
            {
                result += sci.Quantity;
            }
            return result;
        }
    }
}
