using Nop.Core.Domain.Affiliates;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the affiliate model factory
    /// </summary>
    public partial interface IAffiliateModelFactory
    {
        /// <summary>
        /// Prepare affiliate search model
        /// </summary>
        /// <param name="model">Affiliate search model</param>
        /// <returns>Affiliate search model</returns>
        AffiliateSearchModel PrepareAffiliateSearchModel(AffiliateSearchModel model);

        /// <summary>
        /// Prepare paged affiliate list model for the grid
        /// </summary>
        /// <param name="searchModel">Affiliate search model</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAffiliateListGridModel(AffiliateSearchModel searchModel);

        /// <summary>
        /// Prepare affiliate model
        /// </summary>
        /// <param name="model">Affiliate model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Affiliate model</returns>
        AffiliateModel PrepareAffiliateModel(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged affiliated order list model for the grid
        /// </summary>
        /// <param name="searchModel">Affiliated order search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAffiliatedOrderListGridModel(AffiliatedOrderSearchModel searchModel, Affiliate affiliate);

        /// <summary>
        /// Prepare paged affiliated customer list model for the grid
        /// </summary>
        /// <param name="searchModel">Affiliated customer search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAffiliatedCustomerListGridModel(AffiliatedCustomerSearchModel searchModel, Affiliate affiliate);
    }
}