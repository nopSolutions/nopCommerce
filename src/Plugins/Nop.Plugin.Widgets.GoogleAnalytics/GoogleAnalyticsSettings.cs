
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public class GoogleAnalyticsSettings : ISettings
    {
        public string GoogleId { get; set; }
        public string JavaScript { get; set; }
    }
}