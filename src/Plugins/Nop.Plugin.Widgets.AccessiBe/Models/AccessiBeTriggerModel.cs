using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.AccessiBe.Domain;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.AccessiBe.Models;

/// <summary>
/// Represents AccessiBe trigger model
/// </summary>
public record AccessiBeTriggerModel : BaseNopModel, ISettingsModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    public bool DisableBgProcess { get; set; } = false;

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.LeadColor")]
    public string LeadColor { get; set; } = "#146FF8";
    public bool LeadColor_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.StatementLink")]
    public string StatementLink { get; set; } = "";
    public bool StatementLink_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.FooterHtml")]
    public string FooterHtml { get; set; } = "";
    public bool FooterHtml_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.ShowMobile")]
    public bool ShowMobile { get; set; } = false;
    public bool ShowMobile_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.HideTrigger")]
    public bool HideTrigger { get; set; } = false;
    public bool HideTrigger_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.Language")]
    public string Language { get; set; } = "en";
    public bool Language_OverrideForStore { get; set; }
    public SelectList Languages { get; set; } = new(AccessiBeDefaults.SupportedLanuages, "Value", "Key");

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.Position")]
    public TriggerHorizontalPosition Position { get; set; } = TriggerHorizontalPosition.Right;
    public bool Position_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerColor")]
    public string TriggerColor { get; set; } = "#146FF8";
    public bool TriggerColor_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerPositionX")]
    public TriggerHorizontalPosition TriggerPositionX { get; set; } = TriggerHorizontalPosition.Right;
    public bool TriggerPositionX_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerPositionY")]
    public TriggerVerticalPosition TriggerPositionY { get; set; } = TriggerVerticalPosition.Bottom;
    public bool TriggerPositionY_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerShape")]
    public TriggerButtonShape TriggerRadius { get; set; } = TriggerButtonShape.Round;
    public bool TriggerRadius_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerIcon")]
    public TriggerIcon TriggerIcon { get; set; } = TriggerIcon.People;
    public bool TriggerIcon_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerSize")]
    public TriggerButtonSize TriggerSize { get; set; } = TriggerButtonSize.Medium;
    public bool TriggerSize_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerOffsetX")]
    public int TriggerOffsetX { get; set; } = 20;
    public bool TriggerOffsetX_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.AccessiBe.Fields.TriggerOffsetY")]
    public int TriggerOffsetY { get; set; } = 20;
    public bool TriggerOffsetY_OverrideForStore { get; set; }
}
