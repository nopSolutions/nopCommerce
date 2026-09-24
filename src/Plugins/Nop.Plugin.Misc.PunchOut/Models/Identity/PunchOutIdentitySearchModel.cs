using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.PunchOut.Models.Identity;

/// <summary>
/// Represents PunchOutIdentity search model
/// </summary>
public record PunchOutIdentitySearchModel : BaseSearchModel
{
    #region Ctor

    public PunchOutIdentitySearchModel()
    {
        AddIdentity = new PunchOutIdentityModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Identity.Search.Identity")]
    public string Identity { get; set; }

    public bool HideSearchBlock { get; set; }

    public PunchOutIdentityModel AddIdentity { get; set; }

    #endregion
}
