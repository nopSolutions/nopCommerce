using Nop.Plugin.Widgets.AccessiBe.Models;
using Nop.Web.Framework.Models;
using Riok.Mapperly.Abstractions;

namespace Nop.Plugin.Widgets.AccessiBe.Infrastructure;

/// <summary>
/// AccessiBe mapper
/// </summary>
[Mapper]
public partial class AccessiBeMapper
{
    [MapperIgnoreTarget(nameof(BaseNopModel.CustomProperties))]
    [MapperIgnoreTarget(nameof(ISettingsModel.ActiveStoreScopeConfiguration))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerMobileModel.TriggerSize_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerMobileModel.TriggerPositionX_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerMobileModel.TriggerPositionY_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerMobileModel.TriggerOffsetX_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerMobileModel.TriggerOffsetY_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerMobileModel.TriggerRadius_OverrideForStore))]
    public partial AccessiBeTriggerMobileModel Map(AccessiBeMobileSettings source);

    [MapperIgnoreSource(nameof(BaseNopModel.CustomProperties))]
    [MapperIgnoreSource(nameof(ISettingsModel.ActiveStoreScopeConfiguration))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerMobileModel.TriggerSize_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerMobileModel.TriggerPositionX_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerMobileModel.TriggerPositionY_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerMobileModel.TriggerOffsetX_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerMobileModel.TriggerOffsetY_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerMobileModel.TriggerRadius_OverrideForStore))]
    public partial AccessiBeMobileSettings Map(AccessiBeTriggerMobileModel source);

    [MapperIgnoreTarget(nameof(BaseNopModel.CustomProperties))]
    [MapperIgnoreTarget(nameof(ISettingsModel.ActiveStoreScopeConfiguration))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.DisableBgProcess))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.Languages))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.LeadColor_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.StatementLink_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.FooterHtml_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.ShowMobile_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.HideTrigger_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.Language_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.Position_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerColor_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerPositionX_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerPositionY_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerRadius_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerIcon_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerSize_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerOffsetX_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeTriggerModel.TriggerOffsetY_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeSettings.Script))]
    [MapperIgnoreSource(nameof(AccessiBeSettings.WidgetZone))]
    [MapProperty(nameof(AccessiBeSettings.HideMobile), nameof(AccessiBeTriggerModel.ShowMobile), Use = nameof(NegateHideMobile))]
    public partial AccessiBeTriggerModel Map(AccessiBeSettings source);

    [MapperIgnoreSource(nameof(BaseNopModel.CustomProperties))]
    [MapperIgnoreSource(nameof(ISettingsModel.ActiveStoreScopeConfiguration))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.DisableBgProcess))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.Languages))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.LeadColor_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.StatementLink_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.FooterHtml_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.ShowMobile_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.HideTrigger_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.Language_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.Position_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerColor_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerPositionX_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerPositionY_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerRadius_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerIcon_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerSize_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerOffsetX_OverrideForStore))]
    [MapperIgnoreSource(nameof(AccessiBeTriggerModel.TriggerOffsetY_OverrideForStore))]
    [MapperIgnoreTarget(nameof(AccessiBeSettings.Script))]
    [MapperIgnoreTarget(nameof(AccessiBeSettings.WidgetZone))]
    [MapProperty(nameof(AccessiBeTriggerModel.ShowMobile), nameof(AccessiBeSettings.HideMobile), Use = nameof(NegateShowMobile))]
    public partial AccessiBeSettings Map(AccessiBeTriggerModel source);

    private static bool NegateHideMobile(bool hideMobile) => !hideMobile;
    private static bool NegateShowMobile(bool showMobile) => !showMobile;
}