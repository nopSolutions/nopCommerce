using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts;

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
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteDiscountAsync(Discount discount);

    /// <summary>
    /// Gets a discount
    /// </summary>
    /// <param name="discountId">Discount identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount
    /// </returns>
    Task<Discount> GetDiscountByIdAsync(int discountId);

    /// <summary>
    /// Gets all discounts
    /// </summary>
    /// <param name="discountType">Discount type; pass null to load all records</param>
    /// <param name="couponCode">Coupon code to find (exact match); pass null or empty to load all records</param>
    /// <param name="discountName">Discount name; pass null or empty to load all records</param>
    /// <param name="showHidden">A value indicating whether to show expired and not started discounts</param>
    /// <param name="startDateUtc">Discount start date; pass null to load all records</param>
    /// <param name="endDateUtc">Discount end date; pass null to load all records</param>
    /// <param name="isActive">A value indicating whether to get active discounts; "null" to load all discounts; "false" to load only inactive discounts; "true" to load only active discounts</param>
    /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discounts
    /// </returns>
    Task<IList<Discount>> GetAllDiscountsAsync(DiscountType? discountType = null,
        string couponCode = null, string discountName = null, bool showHidden = false,
        DateTime? startDateUtc = null, DateTime? endDateUtc = null, bool? isActive = true, int vendorId = 0);

    /// <summary>
    /// Inserts a discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertDiscountAsync(Discount discount);

    /// <summary>
    /// Updates the discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateDiscountAsync(Discount discount);

    /// <summary>
    /// Gets discounts applied to entity
    /// </summary>
    /// <typeparam name="T">Type based on <see cref="DiscountMapping" /></typeparam>
    /// <param name="entity">Entity which supports discounts (<see cref="IDiscountSupported{T}" />)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of discounts
    /// </returns>
    Task<IList<Discount>> GetAppliedDiscountsAsync<T>(IDiscountSupported<T> entity) where T : DiscountMapping;

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the requirements
    /// </returns>
    Task<IList<DiscountRequirement>> GetAllDiscountRequirementsAsync(int discountId = 0, bool topLevelOnly = false);

    /// <summary>
    /// Get a discount requirement
    /// </summary>
    /// <param name="discountRequirementId">Discount requirement identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<DiscountRequirement> GetDiscountRequirementByIdAsync(int discountRequirementId);

    /// <summary>
    /// Gets child discount requirements
    /// </summary>
    /// <param name="discountRequirement">Parent discount requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task<IList<DiscountRequirement>> GetDiscountRequirementsByParentAsync(DiscountRequirement discountRequirement);

    /// <summary>
    /// Delete discount requirement
    /// </summary>
    /// <param name="discountRequirement">Discount requirement</param>
    /// <param name="recursively">A value indicating whether to recursively delete child requirements</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteDiscountRequirementAsync(DiscountRequirement discountRequirement, bool recursively);

    /// <summary>
    /// Inserts a discount requirement
    /// </summary>
    /// <param name="discountRequirement">Discount requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertDiscountRequirementAsync(DiscountRequirement discountRequirement);

    /// <summary>
    /// Updates a discount requirement
    /// </summary>
    /// <param name="discountRequirement">Discount requirement</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateDiscountRequirementAsync(DiscountRequirement discountRequirement);

    #endregion

    #region Validation
        
    /// <summary>
    /// Validate discount
    /// </summary>
    /// <param name="discount">Discount</param>
    /// <param name="customer">Customer</param>
    /// <param name="couponCodesToValidate">Coupon codes to validate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount validation result
    /// </returns>
    Task<DiscountValidationResult> ValidateDiscountAsync(Discount discount, Customer customer, string[] couponCodesToValidate);

    #endregion

    #region Discount usage history

    /// <summary>
    /// Gets a discount usage history record
    /// </summary>
    /// <param name="discountUsageHistoryId">Discount usage history record identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount usage history
    /// </returns>
    Task<DiscountUsageHistory> GetDiscountUsageHistoryByIdAsync(int discountUsageHistoryId);

    /// <summary>
    /// Gets all discount usage history records
    /// </summary>
    /// <param name="discountId">Discount identifier; null to load all records</param>
    /// <param name="customerId">Customer identifier; null to load all records</param>
    /// <param name="orderId">Order identifier; null to load all records</param>
    /// <param name="includeCancelledOrders">Include cancelled orders</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount usage history records
    /// </returns>
    Task<IPagedList<DiscountUsageHistory>> GetAllDiscountUsageHistoryAsync(int? discountId = null,
        int? customerId = null, int? orderId = null, bool includeCancelledOrders = true,
        int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Insert discount usage history record
    /// </summary>
    /// <param name="discountUsageHistory">Discount usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertDiscountUsageHistoryAsync(DiscountUsageHistory discountUsageHistory);

    /// <summary>
    /// Delete discount usage history record
    /// </summary>
    /// <param name="discountUsageHistory">Discount usage history record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteDiscountUsageHistoryAsync(DiscountUsageHistory discountUsageHistory);

    #endregion
}