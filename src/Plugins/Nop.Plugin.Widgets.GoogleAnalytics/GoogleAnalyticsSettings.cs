using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class GoogleAnalyticsSettings : ISettings
    {
        public string GoogleId { get; set; }
        public string TrackingScript { get; set; }
        public bool EnableEcommerce { get; set; }
        public bool IncludingTax { get; set; }
        public string WidgetZone { get; set; }
    }
}