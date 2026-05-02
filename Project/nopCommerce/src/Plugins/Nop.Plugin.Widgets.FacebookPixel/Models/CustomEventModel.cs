using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a custom event model
/// </summary>
public record CustomEventModel : BaseNopModel
{
    #region Ctor

    public CustomEventModel()
    {
        WidgetZonesIds = new List<int>();
        WidgetZones = new List<string>();
        AvailableWidgetZones = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    public int ConfigurationId { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.EventName")]
    public string EventName { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.CustomEvents.Fields.WidgetZones")]
    public IList<int> WidgetZonesIds { get; set; }
    public IList<string> WidgetZones { get; set; }
    public IList<SelectListItem> AvailableWidgetZones { get; set; }
    public string WidgetZonesName { get; set; }

    #endregion
}