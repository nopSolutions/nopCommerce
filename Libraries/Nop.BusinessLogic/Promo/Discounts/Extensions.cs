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
using System.Text;


namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the collection contains the specified discount.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="discountName">The discount name</param>
        /// <returns>true if the collection contains a discount with the specified name; otherwise, false.</returns>
        public static bool ContainsDiscount(this List<Discount> source,
            string discountName)
        {
            bool result = false;
            foreach (var discount in source)
                if (discount.Name == discountName)
                {
                    result = true;
                    break;
                }
            return result;
        }

        /// <summary>
        /// Finds the specified discount.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="couponCode">The coupon code</param>
        /// <returns>Found discount</returns>
        public static Discount FindByCouponCode(this List<Discount> source,
            string couponCode)
        {
            if (String.IsNullOrEmpty(couponCode))
                return null;

            foreach (var discount in source)
                if (discount.CouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    return discount;

            return null;
        }
    }
}
