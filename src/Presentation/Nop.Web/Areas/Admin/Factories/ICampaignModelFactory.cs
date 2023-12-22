using Nop.Core.Domain.Messages;
using Nop.Web.Areas.Admin.Models.Messages;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the campaign model factory
/// </summary>
public partial interface ICampaignModelFactory
{
    /// <summary>
    /// Prepare campaign search model
    /// </summary>
    /// <param name="searchModel">Campaign search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaign search model
    /// </returns>
    Task<CampaignSearchModel> PrepareCampaignSearchModelAsync(CampaignSearchModel searchModel);

    /// <summary>
    /// Prepare paged campaign list model
    /// </summary>
    /// <param name="searchModel">Campaign search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaign list model
    /// </returns>
    Task<CampaignListModel> PrepareCampaignListModelAsync(CampaignSearchModel searchModel);

    /// <summary>
    /// Prepare campaign model
    /// </summary>
    /// <param name="model">Campaign model</param>
    /// <param name="campaign">Campaign</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaign model
    /// </returns>
    Task<CampaignModel> PrepareCampaignModelAsync(CampaignModel model, Campaign campaign, bool excludeProperties = false);
}