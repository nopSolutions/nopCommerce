using Nop.Core.Configuration;
using Nop.Plugin.Widgets.AccessiBe.Domain;

namespace Nop.Plugin.Widgets.AccessiBe;

/// <summary>
/// Represents trigger button settings for mobile
/// </summary>
public class AccessiBeMobileSettings : ISettings
{
    public TriggerButtonSize TriggerSize { get; set; }
    public TriggerHorizontalPosition TriggerPositionX { get; set; }
    public TriggerVerticalPosition TriggerPositionY { get; set; }
    public int TriggerOffsetX { get; set; }
    public int TriggerOffsetY { get; set; }
    public TriggerButtonShape TriggerRadius { get; set; }
}
