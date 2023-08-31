using AbcWarehouse.Plugin.Widgets.GA4.Models;
using Nop.Core.Configuration;

namespace AbcWarehouse.Plugin.Widgets.GA4
{
    public class GA4Settings : ISettings
    {
        public string GoogleTag { get; set; }
        public bool IsDebugMode { get; set; }
    }
}