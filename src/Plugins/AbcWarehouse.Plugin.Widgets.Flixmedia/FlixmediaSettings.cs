using AbcWarehouse.Plugin.Widgets.Flixmedia.Models;
using Nop.Core.Configuration;
using Nop.Web.Framework.Infrastructure;

namespace AbcWarehouse.Plugin.Widgets.Flixmedia
{
    public class FlixmediaSettings : ISettings
    {
        public string FlixID { get; set; }
        public string WidgetZone { get; set; }

        public static FlixmediaSettings DefaultValues()
        {
            return new FlixmediaSettings()
            {
                WidgetZone = PublicWidgetZones.ProductDetailsEssentialBottom
            };
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FlixID);
        }
    }
}