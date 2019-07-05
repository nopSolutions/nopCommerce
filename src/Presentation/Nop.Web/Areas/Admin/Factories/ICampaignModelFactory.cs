using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the campaign model factory
    /// </summary>
    public partial interface ICampaignModelFactory
    {
        /// <summary>
        /// Prepare campaign search model
        /// </summary>
        /// <param name="searchModel">Campaign search model</param>
        /// <returns>Campaign search model</returns>
        CampaignSearchModel PrepareCampaignSearchModel(CampaignSearchModel searchModel);

        /// <summary>
        /// Prepare paged campaign list model
        /// </summary>
        /// <param name="searchModel">Campaign search model</param>
        /// <returns>Campaign list model</returns>
        CampaignListModel PrepareCampaignListModel(CampaignSearchModel searchModel);

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