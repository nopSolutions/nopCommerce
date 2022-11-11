using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a campaign list model
    /// </summary>
    public partial record CampaignListModel : BasePagedListModel<CampaignModel>
    {
    }
}