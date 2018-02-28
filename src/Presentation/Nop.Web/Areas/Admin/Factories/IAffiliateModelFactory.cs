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
        /// Prepare affiliate list model
        /// </summary>
        /// <param name="model">Affiliate list model</param>
        /// <returns>Affiliate list model</returns>
        AffiliateListModel PrepareAffiliateListModel(AffiliateListModel model);

        /// <summary>
        /// Prepare paged affiliate list model for the grid
        /// </summary>
        /// <param name="listModel">Affiliate list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAffiliateListGridModel(AffiliateListModel listModel, DataSourceRequest command);

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
        /// <param name="listModel">Affiliated order list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAffiliatedOrderListGridModel(AffiliateModel.AffiliatedOrderListModel listModel,
            DataSourceRequest command, Affiliate affiliate);

        /// <summary>
        /// Prepare paged affiliated customer list model for the grid
        /// </summary>
        /// <param name="command">Pagination parameters</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareAffiliatedCustomerListGridModel(DataSourceRequest command, Affiliate affiliate);
    }
}