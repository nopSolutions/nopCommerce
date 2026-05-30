using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;


/// <summary>
/// Represents a private messages settings model
/// </summary>
public partial record PrivateMessageSettingsModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.AllowPrivateMessages")]
    public bool AllowPrivateMessages { get; set; }
    public bool AllowPrivateMessages_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.ShowAlertForPM")]
    public bool ShowAlertForPM { get; set; }
    public bool ShowAlertForPM_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.CustomerUser.NotifyAboutPrivateMessages")]
    public bool NotifyAboutPrivateMessages { get; set; }
    public bool NotifyAboutPrivateMessages_OverrideForStore { get; set; }
}
