using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a copy campaign model
/// </summary>
public partial record CopyCampaignModel : BaseNopModel
{
    #region Properties

    public int OriginalCampaignId { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Copy.Name")]
    public string Name { get; set; }

    #endregion
}
