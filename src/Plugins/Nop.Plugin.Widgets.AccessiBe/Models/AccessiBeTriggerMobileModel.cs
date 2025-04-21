using Nop.Plugin.Widgets.AccessiBe.Domain;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.AccessiBe.Models;

/// <summary>
/// Represents AccessiBe trigger model for mobile devices
/// </summary>
public record AccessiBeTriggerMobileModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerSize")]
    public TriggerButtonSize TriggerSize { get; set; } = TriggerButtonSize.Small;
    public bool TriggerSize_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerPositionX")]
    public TriggerHorizontalPosition TriggerPositionX { get; set; } = TriggerHorizontalPosition.Right;
    public bool TriggerPositionX_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerPositionY")]
    public TriggerVerticalPosition TriggerPositionY { get; set; } = TriggerVerticalPosition.Bottom;
    public bool TriggerPositionY_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerOffsetX")]
    public int TriggerOffsetX { get; set; } = 10;
    public bool TriggerOffsetX_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerOffsetY")]
    public int TriggerOffsetY { get; set; } = 10;
    public bool TriggerOffsetY_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerShape")]
    public TriggerButtonShape TriggerRadius { get; set; } = TriggerButtonShape.Round;
    public bool TriggerRadius_OverrideForStore { get; set; }
}
