using Nop.Core.Configuration;

namespace Nop.Plugin.LiveChat.LivePerson
{
    public class LivePersonChatSettings : ISettings
    {
        public string ButtonCode { get; set; }
        public string MonitoringCode { get; set; }
    }
}
