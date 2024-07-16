using AbcWarehouse.Plugin.Widgets.UniFi.Models;
using Nop.Core.Configuration;

namespace AbcWarehouse.Plugin.Widgets.UniFi
{
    public class UniFiSettings : ISettings
    {
        public bool IsEnabled { get; set; }
        public string PartnerId { get; set; }
        public bool UseIntegration { get; set; }
    }
}