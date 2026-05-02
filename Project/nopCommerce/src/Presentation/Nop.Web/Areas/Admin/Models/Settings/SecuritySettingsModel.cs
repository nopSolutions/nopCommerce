using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a security settings model
/// </summary>
public partial record SecuritySettingsModel : BaseNopModel, ISettingsModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.EncryptionKey")]
    public string EncryptionKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AdminAreaAllowedIpAddresses")]
    public string AdminAreaAllowedIpAddresses { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.HoneypotEnabled")]
    public bool HoneypotEnabled { get; set; }

    #endregion
}