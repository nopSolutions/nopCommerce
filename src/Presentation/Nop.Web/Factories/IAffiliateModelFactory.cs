using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Affiliate;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the interface of the affiliate model factory
/// </summary>
public partial interface IAffiliateModelFactory
{
    /// <summary>
    /// Prepares the apply affiliate model
    /// </summary>
    /// <param name="model">An apply affiliate model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the apply affiliate model
    /// </returns>
    Task<ApplyAffiliateModel> PrepareApplyAffiliateModelAsync(ApplyAffiliateModel model);

    /// <summary>
    /// Prepares the affiliate model
    /// </summary>
    /// <param name="affiliate">The affiliate</param>
    /// <param name="model">An affiliate model</param>
    /// <param name="limit">The order history period</param>
    /// <param name="page">The page number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate model
    /// </returns>
    Task<AffiliateModel> PrepareAffiliateModelAsync(Affiliate affiliate, AffiliateModel model, OrderHistoryPeriods limit, int? page);
}