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
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount service interface
    /// </summary>
    public partial interface IDiscountService
    {
        /// <summary>
        /// Delete discount
        /// </summary>
        /// <param name="discount">Discount</param>
        void DeleteDiscount(Discount discount);

        /// <summary>
        /// Gets a discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Discount</returns>
        Discount GetDiscountById(int discountId);

        /// <summary>
        /// Gets all discounts
        /// </summary>
        /// <param name="discountType">Discount type; null to load all discount</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Discount collection</returns>
        IList<Discount> GetAllDiscounts(DiscountType? discountType, bool showHidden = false);

        /// <summary>
        /// Inserts a discount
        /// </summary>
        /// <param name="discount">Discount</param>
        void InsertDiscount(Discount discount);

        /// <summary>
        /// Updates the discount
        /// </summary>
        /// <param name="discount">Discount</param>
        void UpdateDiscount(Discount discount);

        /// <summary>
        /// Load discount requirement rule by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found discount requirement rule</returns>
        IDiscountRequirementRule LoadDiscountRequirementRuleBySystemName(string systemName);

        /// <summary>
        /// Load all discount requirement rules
        /// </summary>
        /// <returns>Discount requirement rules</returns>
        IList<IDiscountRequirementRule> LoadAllDiscountRequirementRules();
        
        /// <summary>
        /// Check discount requirements
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        bool IsDiscountValid(Discount discount, Customer customer);

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="discounts">Discounts to analyze</param>
        /// <param name="amount">Amount</param>
        /// <returns>Preferred discount</returns>
        Discount GetPreferredDiscount(IList<Discount> discounts, decimal amount);
    }
}
