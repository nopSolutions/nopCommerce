using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the campaign model factory
    /// </summary>
    public partial interface ICampaignModelFactory
    {
        /// <summary>
        /// Prepare campaign list model
        /// </summary>
        /// <param name="model">Campaign list model</param>
        /// <returns>Campaign list model</returns>
        CampaignListModel PrepareCampaignListModel(CampaignListModel model);

        /// <summary>
        /// Prepare paged campaign list model for the grid
        /// </summary>
        /// <param name="listModel">Campaign list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        DataSourceResult PrepareCampaignListGridModel(CampaignListModel listModel, DataSourceRequest command);

        /// <summary>
        /// Prepare campaign model
        /// </summary>
        /// <param name="model">Campaign model</param>
        /// <param name="campaign">Campaign</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Campaign model</returns>
        CampaignModel PrepareCampaignModel(CampaignModel model, Campaign campaign, bool excludeProperties = false);
    }
}