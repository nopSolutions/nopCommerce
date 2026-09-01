using Nop.Core.Domain.Affiliates;
using Nop.Web.Areas.Admin.Models.Affiliates;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the affiliate model factory
/// </summary>
public partial interface IAffiliateModelFactory
{
    /// <summary>
    /// Prepare affiliate customer search model
    /// </summary>
    /// <param name="searchModel">Affiliate customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate customer search model
    /// </returns>
    Task<AffiliateCustomerSearchModel> PrepareAffiliateCustomerSearchModelAsync(AffiliateCustomerSearchModel searchModel);

    /// <summary>
    /// Prepare affiliate search model
    /// </summary>
    /// <param name="searchModel">Affiliate search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate search model
    /// </returns>
    Task<AffiliateSearchModel> PrepareAffiliateSearchModelAsync(AffiliateSearchModel searchModel);

    /// <summary>
    /// Prepare paged affiliate list model
    /// </summary>
    /// <param name="searchModel">Affiliate search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate list model
    /// </returns>
    Task<AffiliateListModel> PrepareAffiliateListModelAsync(AffiliateSearchModel searchModel);

    /// <summary>
    /// Prepare affiliate model
    /// </summary>
    /// <param name="model">Affiliate model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate model
    /// </returns>
    Task<AffiliateModel> PrepareAffiliateModelAsync(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false);

    /// <summary>
    /// Prepare paged affiliated order list model
    /// </summary>
    /// <param name="searchModel">Affiliated order search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated order list model
    /// </returns>
    Task<AffiliatedOrderListModel> PrepareAffiliatedOrderListModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate);

    /// <summary>
    /// Prepare affiliate commission list model
    /// </summary>
    /// <param name="searchModel">Affiliated commission search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commission list model
    /// </returns>
    Task<AffiliateCommissionListModel> PrepareAffiliateCommissionListModelAsync(AffiliateCommissionSearchModel searchModel, Affiliate affiliate);
    
    /// <summary>
    /// Prepare paged affiliated customer list model
    /// </summary>
    /// <param name="searchModel">Affiliated customer search model</param>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliated customer list model
    /// </returns>
    Task<AffiliatedCustomerListModel> PrepareAffiliatedCustomerListModelAsync(AffiliatedCustomerSearchModel searchModel,
        Affiliate affiliate);

    /// <summary>
    /// Prepare paged affiliate customer list model
    /// </summary>
    /// <param name="searchModel">Affiliate customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate customer list model
    /// </returns>
    Task<AffiliateCustomerListModel> PrepareAffiliateCustomerListModelAsync(AffiliateCustomerSearchModel searchModel);

    /// <summary>
    /// Prepare affiliate commission editor model
    /// </summary>
    /// <param name="affiliateCommissionEditModel">Affiliate commission editor model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliate commission editor model
    /// </returns>
    Task<AffiliateCommissionEditModel> PrepareAffiliateCommissionEditModelAsync(AffiliateCommissionEditModel affiliateCommissionEditModel);
}