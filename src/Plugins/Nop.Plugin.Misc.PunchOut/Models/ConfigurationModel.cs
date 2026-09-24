using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.PunchOut.Models.Identity;
using Nop.Plugin.Misc.PunchOut.Models.Log;
using Nop.Plugin.Misc.PunchOut.Models.Session;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.PunchOut.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Ctor

    public ConfigurationModel()
    {
        PunchOutLogSearchModel = new PunchOutLogSearchModel();
        PunchOutIdentitySearchModel = new PunchOutIdentitySearchModel();
        PunchOutSessionSearchModel = new PunchOutSessionSearchModel();
        SelectedCustomerRoleIds = new List<int>();
        UnavailableCustomerRoles = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Configuration.IsActive")]
    public bool IsActive { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Configuration.TimeToExpire")]
    public int TimeToExpire { get; set; }

    public IList<SelectListItem> UnavailableCustomerRoles { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Configuration.CustomerRoles")]
    public IList<int> SelectedCustomerRoleIds { get; set; }

    public bool HideGeneralBlock { get; set; }
    public bool HideIdentityBlock { get; set; }
    public bool HideSessionBlock { get; set; }
    public bool HideLogBlock { get; set; }

    public PunchOutLogSearchModel PunchOutLogSearchModel { get; set; }

    public PunchOutIdentitySearchModel PunchOutIdentitySearchModel { get; set; }

    public PunchOutSessionSearchModel PunchOutSessionSearchModel { get; set; }

    #endregion
}
