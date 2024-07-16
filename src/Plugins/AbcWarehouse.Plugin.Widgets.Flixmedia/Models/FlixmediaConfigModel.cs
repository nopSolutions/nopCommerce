using Nop.Web.Framework.Mvc.ModelBinding;

namespace AbcWarehouse.Plugin.Widgets.Flixmedia.Models
{
    public class FlixmediaConfigModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName(FlixmediaLocales.FlixID)]
        public string FlixID { get; set; }
        public bool FlixID_OverrideForStore { get; set; }

        [NopResourceDisplayName(FlixmediaLocales.WidgetZone)]
        public string WidgetZone { get; set; }
        public bool WidgetZone_OverrideForStore { get; set; }
    }
}