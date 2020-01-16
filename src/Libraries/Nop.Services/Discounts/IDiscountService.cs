using System;
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
        /// <param name="discountType">Discount type; pass null to load all records</param>
        /// <param name="couponCode">Coupon code to find (exact match); pass null or empty to load all records</param>
        /// <param name="discountName">Discount name; pass null or empty to load all records</param>
        /// <param name="showHidden">A value indicating whether to show expired and not started discounts</param>
        /// <param name="startDateUtc">Discount start date; pass null to load all records</param>
        /// <param name="endDateUtc">Discount end date; pass null to load all records</param>
        /// <returns>Discounts</returns>
        IList<Discount> GetAllDiscounts(DiscountType? discountType = null,
            string couponCode = null, string discountName = null, bool showHidden = false,
            DateTime? startDateUtc = null, DateTime? endDateUtc = null);

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
        /// Gets discounts applied to entity
        /// </summary>
        /// <typeparam name="T">Type based on <see cref="DiscountMapping" /></typeparam>
        /// <param name="entity">Entity which supports discounts (<see cref="IDiscountSupported{T}" />)</param>
        /// <returns>List of discounts</returns>
        IList<Discount> GetAppliedDiscounts<T>(IDiscountSupported<T> entity) where T : DiscountMapping;

        #endregion

        #region Discounts (caching)

        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="amount">Amount</param>
        /// <returns>The discount amount</returns>
        decimal GetDiscountAmount(Discount discount, decimal amount);

        /// <summary>
        /// Get preferred discount (with maximum discount value)
        /// </summary>
        /// <param name="discounts">A list of discounts to check</param>
        /// <param name="amount">Amount (initial value)</param>
        /// <param name="discountAmount">Discount amount</param>
        /// <returns>Preferred discount</returns>
        List<Discount> GetPreferredDiscount(IList<Discount> discounts,
            decimal amount, out decimal discountAmount);

        /// <summary>
        /// Check whether a list of discounts already contains a certain discount instance
        /// </summary>
        /// <param name="discounts">A list of discounts</param>
        /// <param name="discount">Discount to check</param>
        /// <returns>Result</returns>
        bool ContainsDiscount(IList<Discount> discounts, Discount discount);

        #endregion

        #region Discount requirements

        /// <summary>
        /// Get all discount requirements
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="topLevelOnly">Whether to load top-level requirements only (without parent identifier)</param>
        /// <returns>Requirements</returns>
        IList<DiscountRequirement> GetAllDiscountRequirements(int discountId = 0, bool topLevelOnly = false);

        /// <summary>
        /// Get a discount requirement
        /// </summary>
        /// <param name="discountRequirementId">Discount requirement identifier</param>
        DiscountRequirement GetDiscountRequirementById(int discountRequirementId);

        /// <summary>
        /// Gets child discount requirements
        /// </summary>
        /// <param name="discountRequirement">Parent discount requirement</param>
        IList<DiscountRequirement> GetDiscountRequirementsByParent(DiscountRequirement discountRequirement);

        /// <summary>
        /// Delete discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        /// <param name="recursively">A value indicating whether to recursively delete child requirements</param>
        void DeleteDiscountRequirement(DiscountRequirement discountRequirement, bool recursively);

        /// <summary>
        /// Inserts a discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        void InsertDiscountRequirement(DiscountRequirement discountRequirement);

        /// <summary>
        /// Updates a discount requirement
        /// </summary>
        /// <param name="discountRequirement">Discount requirement</param>
        void UpdateDiscountRequirement(DiscountRequirement discountRequirement);

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