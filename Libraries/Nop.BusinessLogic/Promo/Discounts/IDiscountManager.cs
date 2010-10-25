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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts
{
    /// <summary>
    /// Discount manager interface
    /// </summary>
    public partial interface IDiscountManager
    {
        #region Discounts

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="discounts">Discounts to analyze</param>
        /// <param name="amount">Amount</param>
        /// <returns>Preferred discount</returns>
        Discount GetPreferredDiscount(List<Discount> discounts,
            decimal amount);

        /// <summary>
        /// Gets a discount
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>Discount</returns>
        Discount GetDiscountById(int discountId);

        /// <summary>
        /// Marks discount as deleted
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        void MarkDiscountAsDeleted(int discountId);

        /// <summary>
        /// Get a value indicating whether discounts that require coupon code exist
        /// </summary>
        /// <returns>A value indicating whether discounts that require coupon code exist</returns>
        bool HasDiscountsWithCouponCode();

        /// <summary>
        /// Gets all discounts
        /// </summary>
        /// <param name="discountType">Discount type; null to load all discount</param>
        /// <returns>Discount collection</returns>
        List<Discount> GetAllDiscounts(DiscountTypeEnum? discountType);

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
        /// Adds a discount to a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void AddDiscountToProductVariant(int productVariantId, int discountId);

        /// <summary>
        /// Removes a discount from a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void RemoveDiscountFromProductVariant(int productVariantId, int discountId);

        /// <summary>
        /// Gets a discount collection of a product variant
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Discount collection</returns>
        List<Discount> GetDiscountsByProductVariantId(int productVariantId);

        /// <summary>
        /// Adds a discount to a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void AddDiscountToCategory(int categoryId, int discountId);

        /// <summary>
        /// Removes a discount from a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void RemoveDiscountFromCategory(int categoryId, int discountId);

        /// <summary>
        /// Gets a discount collection of a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Discount collection</returns>
        List<Discount> GetDiscountsByCategoryId(int categoryId);

        /// <summary>
        /// Adds a discount requirement
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void AddDiscountRestriction(int productVariantId, int discountId);

        /// <summary>
        /// Removes discount requirement
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="discountId">Discount identifier</param>
        void RemoveDiscountRestriction(int productVariantId, int discountId);

        #endregion

        #region Etc

        /// <summary>
        /// Gets all discount requirements
        /// </summary>
        /// <returns>Discount requirement collection</returns>
        List<DiscountRequirement> GetAllDiscountRequirements();

        /// <summary>
        /// Gets all discount types
        /// </summary>
        /// <returns>Discount type collection</returns>
        List<DiscountType> GetAllDiscountTypes();
        
        /// <summary>
        /// Gets all discount limitations
        /// </summary>
        /// <returns>Discount limitation collection</returns>
        List<DiscountLimitation> GetAllDiscountLimitations();

        #endregion

        #region Discount History

        /// <summary>
        /// Deletes a discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history entry identifier</param>
        void DeleteDiscountUsageHistory(int discountUsageHistoryId);

        /// <summary>
        /// Gets a discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history entry identifier</param>
        /// <returns>Discount usage history entry</returns>
        DiscountUsageHistory GetDiscountUsageHistoryById(int discountUsageHistoryId);

        /// <summary>
        /// Gets all discount usage history entries
        /// </summary>
        /// <param name="discountId">Discount type identifier; null to load all</param>
        /// <param name="customerId">Customer identifier; null to load all</param>
        /// <param name="orderId">Order identifier; null to load all</param>
        /// <returns>Discount usage history entries</returns>
        List<DiscountUsageHistory> GetAllDiscountUsageHistoryEntries(int? discountId,
            int? customerId, int? orderId);

        /// <summary>
        /// Inserts a discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history entry</param>
        void InsertDiscountUsageHistory(DiscountUsageHistory discountUsageHistory);

        /// <summary>
        /// Updates the discount usage history entry
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history entry</param>
        void UpdateDiscountUsageHistory(DiscountUsageHistory discountUsageHistory);

        #endregion
    }
}
