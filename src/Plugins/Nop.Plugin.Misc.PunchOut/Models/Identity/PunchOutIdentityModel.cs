using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.PunchOut.Models.Identity;

/// <summary>
/// Represents a punch out identity model
/// </summary>
public record PunchOutIdentityModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Identity.Identity")]
    public string Identity { get; set; }

    [NopResourceDisplayName("Plugins.Misc.PunchOut.Identity.SharedSecret")]
    public string SharedSecret { get; set; }

    #endregion
}
