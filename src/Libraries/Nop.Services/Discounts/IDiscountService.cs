using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount service interface
    /// </summary>
    public partial interface IDiscountService
    {
        #region Discounts

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
        /// <param name="couponCode">Coupon code to find (exact match)</param>
        /// <param name="discountName">Discount name</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Discounts</returns>
        IList<Discount> GetAllDiscounts(DiscountType? discountType,
            string couponCode = "", string discountName = "", bool showHidden = false);

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
        /// Get discount by coupon code
        /// </summary>
        /// <param name="couponCode">CouponCode</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Discount</returns>
        Discount GetDiscountByCouponCode(string couponCode, bool showHidden = false);

        #endregion

        #region Discounts (caching)

        /// <summary>
        /// Gets all discounts (cachable models)
        /// </summary>
        /// <param name="discountType">Discount type; null to load all discount</param>
        /// <param name="couponCode">Coupon code to find (exact match)</param>
        /// <param name="discountName">Discount name</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Discounts</returns>
        IList<DiscountForCaching> GetAllDiscountsForCaching(DiscountType? discountType,
            string couponCode = "", string discountName = "", bool showHidden = false);
        
        /// <summary>
        /// Get category identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Category identifiers</returns>
        IList<int> GetAppliedCategoryIds(DiscountForCaching discount, Customer customer);

        /// <summary>
        /// Get manufacturer identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Manufacturer identifiers</returns>
        IList<int> GetAppliedManufacturerIds(DiscountForCaching discount, Customer customer);

        #endregion


        #region Discount requirements

        /// <summary>
        /// Get all discount requirements
        /// </summary>
        /// <param name="discountId">Discont identifier</param>
        /// <returns>Requirements</returns>
        IList<DiscountRequirement> GetAllDiscountRequirements(int discountId = 0);

        /// <summary>
        /// Delete discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        void DeleteDiscountRequirement(DiscountRequirement discountRequirement);

        /// <summary>
        /// Load discount requirement rule by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found discount requirement rule</returns>
        IDiscountRequirementRule LoadDiscountRequirementRuleBySystemName(string systemName);

        /// <summary>
        /// Load all discount requirement rules
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <returns>Discount requirement rules</returns>
        IList<IDiscountRequirementRule> LoadAllDiscountRequirementRules(Customer customer = null);

        #endregion

        #region Validation

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discount validation result</returns>
        DiscountValidationResult ValidateDiscount(Discount discount, Customer customer);

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <param name="couponCodesToValidate">Coupon codes to validate</param>
        /// <returns>Discount validation result</returns>
        DiscountValidationResult ValidateDiscount(Discount discount, Customer customer, string[] couponCodesToValidate);

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discount validation result</returns>
        DiscountValidationResult ValidateDiscount(DiscountForCaching discount, Customer customer);

        /// <summary>
        /// Validate discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="customer">Customer</param>
        /// <param name="couponCodesToValidate">Coupon codes to validate</param>
        /// <returns>Discount validation result</returns>
        DiscountValidationResult ValidateDiscount(DiscountForCaching discount, Customer customer, string[] couponCodesToValidate);

        #endregion

        #region Discount usage history

        /// <summary>
        /// Gets a discount usage history record
        /// </summary>
        /// <param name="discountUsageHistoryId">Discount usage history record identifier</param>
        /// <returns>Discount usage history</returns>
        DiscountUsageHistory GetDiscountUsageHistoryById(int discountUsageHistoryId);
        
        /// <summary>
        /// Gets all discount usage history records
        /// </summary>
        /// <param name="discountId">Discount identifier; null to load all records</param>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <param name="orderId">Order identifier; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Discount usage history records</returns>
        IPagedList<DiscountUsageHistory> GetAllDiscountUsageHistory(int? discountId = null,
            int? customerId = null, int? orderId = null, 
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Insert discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        void InsertDiscountUsageHistory(DiscountUsageHistory discountUsageHistory);
        
        /// <summary>
        /// Update discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        void UpdateDiscountUsageHistory(DiscountUsageHistory discountUsageHistory);

        /// <summary>
        /// Delete discount usage history record
        /// </summary>
        /// <param name="discountUsageHistory">Discount usage history record</param>
        void DeleteDiscountUsageHistory(DiscountUsageHistory discountUsageHistory);

        #endregion
    }
}
