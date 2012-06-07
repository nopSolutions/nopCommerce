
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.LivePersonChat
{
    public class LivePersonChatSettings : ISettings
    {
        public string ButtonCode { get; set; }
        public string MonitoringCode { get; set; }

        public string WidgetZone { get; set; }
    }
}