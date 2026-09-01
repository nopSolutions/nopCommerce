using Nop.Core;
using Nop.Core.Domain.Affiliates;

namespace Nop.Services.Affiliates;

/// <summary>
/// Affiliate service interface
/// </summary>
public partial interface IAffiliateService
{
    /// <summary>
    /// Gets an affiliate by affiliate identifier
    /// </summary>
    /// <param name="affiliateId">Affiliate identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    Task<Affiliate> GetAffiliateByIdAsync(int affiliateId);

    /// <summary>
    /// Gets an affiliate by customer identifier
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    Task<Affiliate> GetAffiliateByCustomerIdAsync(int customerId);

    /// <summary>
    /// Gets an affiliate by friendly URL name
    /// </summary>
    /// <param name="friendlyUrlName">Friendly URL name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate
    /// </returns>
    Task<Affiliate> GetAffiliateByFriendlyUrlNameAsync(string friendlyUrlName);

    /// <summary>
    /// Marks affiliate as deleted 
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteAffiliateAsync(Affiliate affiliate);

    /// <summary>
    /// Gets all affiliates
    /// </summary>
    /// <param name="friendlyUrlName">Friendly URL name; null to load all records</param>
    /// <param name="firstName">First name; null to load all records</param>
    /// <param name="lastName">Last name; null to load all records</param>
    /// <param name="loadOnlyWithOrders">Value indicating whether to load affiliates only with orders placed (by affiliated customers)</param>
    /// <param name="ordersCreatedFromUtc">Orders created date from (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
    /// <param name="ordersCreatedToUtc">Orders created date to (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliates
    /// </returns>
    Task<IPagedList<Affiliate>> GetAllAffiliatesAsync(string friendlyUrlName = null,
        string firstName = null, string lastName = null,
        bool loadOnlyWithOrders = false,
        DateTime? ordersCreatedFromUtc = null, DateTime? ordersCreatedToUtc = null,
        int pageIndex = 0, int pageSize = int.MaxValue,
        bool showHidden = false);

    /// <summary>
    /// Inserts an affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertAffiliateAsync(Affiliate affiliate);

    /// <summary>
    /// Updates the affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAffiliateAsync(Affiliate affiliate);

    /// <summary>
    /// Get full name
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate full name
    /// </returns>
    Task<string> GetAffiliateFullNameAsync(Affiliate affiliate);

    /// <summary>
    /// Generate affiliate URL
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated affiliate URL
    /// </returns>
    Task<string> GenerateUrlAsync(Affiliate affiliate);

    /// <summary>
    /// Validate friendly URL name
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <param name="friendlyUrlName">Friendly URL name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid friendly name
    /// </returns>
    Task<string> ValidateFriendlyUrlNameAsync(Affiliate affiliate, string friendlyUrlName);

    /// <summary>
    /// Inserts an affiliate commission
    /// </summary>
    /// <param name="commission">Affiliate commission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertAffiliateCommission(AffiliateCommission commission);

    /// <summary>
    /// Gets the affiliate commission by order identifiers
    /// </summary>
    /// <param name="commissionIds">The list of affiliate commission identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of affiliate commissions
    /// </returns>
    Task<IList<AffiliateCommission>> GetAffiliateCommissionsByIdsAsync(int[] commissionIds);

    /// <summary>
    /// Gets an affiliate commission by affiliate identifier
    /// </summary>
    /// <param name="commissionId">Affiliate commission identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commission
    /// </returns>
    Task<AffiliateCommission> GetAffiliateCommissionByIdAsync(int commissionId);

    /// <summary>
    /// Updates the affiliate commission
    /// </summary>
    /// <param name="commission">Affiliate commission</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAffiliateCommissionAsync(AffiliateCommission commission);

    /// <summary>
    /// Gets all affiliate commissions
    /// </summary>
    /// <param name="affiliateId">Affiliate identifier</param>
    /// <param name="createdFromUtc">Created from UTC</param>
    /// <param name="createdToUtc">Created to UTC</param>
    /// <param name="commissionStatus">Commission status identifier</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commissions
    /// </returns>
    Task<IPagedList<AffiliateCommission>> GetAllCommissionsAsync(int affiliateId, 
        DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null,
        int? commissionStatus = null,
        int pageIndex = 0, 
        int pageSize = int.MaxValue, 
        bool getOnlyTotalCount = false);

    /// <summary>
    /// Delete affiliate commissions
    /// </summary>
    /// <param name="commission">Commission to delete</param>
    /// <returns> A task that represents the asynchronous operation</returns>
    Task DeleteAffiliateCommissionAsync(AffiliateCommission commission);
}