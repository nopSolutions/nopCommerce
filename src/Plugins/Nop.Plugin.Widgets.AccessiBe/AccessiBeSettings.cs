using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Plugin.Widgets.AccessiBe.Domain;

namespace Nop.Plugin.Widgets.AccessiBe;

/// <summary>
/// Represents plugin settings
/// </summary>
public class AccessiBeSettings : ISettings
{
    /// <summary>
    /// Gets or sets an installation script
    /// </summary>
    [JsonIgnore]
    public string Script { get; set; }

    /// <summary>
    /// Gets or sets a widget zone name to place a widget
    /// </summary>
    [JsonIgnore]
    public string WidgetZone { get; set; }

    #region Trigger button settings

    public string LeadColor { get; set; }
    public string StatementLink { get; set; }
    public string FooterHtml { get; set; }
    public bool HideMobile { get; set; }
    public bool HideTrigger { get; set; }
    public string Language { get; set; }
    public TriggerHorizontalPosition Position { get; set; }
    public string TriggerColor { get; set; }
    public TriggerHorizontalPosition TriggerPositionX { get; set; }
    public TriggerVerticalPosition TriggerPositionY { get; set; }
    public TriggerButtonShape TriggerRadius { get; set; }
    public TriggerIcon TriggerIcon { get; set; }
    public TriggerButtonSize TriggerSize { get; set; }
    public int TriggerOffsetX { get; set; }
    public int TriggerOffsetY { get; set; }

    #endregion
}