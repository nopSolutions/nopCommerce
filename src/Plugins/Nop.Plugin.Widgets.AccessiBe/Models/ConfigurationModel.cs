using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.AccessiBe.Models;

/// <summary>
/// Represents configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.Enabled")]
    public bool Enabled { get; set; }
    public bool Enabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerButton")]
    public AccessiBeTriggerModel TriggerModel { get; set; } = new();

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerButtonMobile")]
    public AccessiBeTriggerMobileModel TriggerMobileModel { get; set; } = new();

    public bool ScriptIsCustomized { get; set; }

    #endregion
}