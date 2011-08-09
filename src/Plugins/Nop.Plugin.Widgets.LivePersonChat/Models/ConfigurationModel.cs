using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.LivePersonChat.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Widgets.LivePersonChat.ButtonCode")]
        [AllowHtml]
        public string ButtonCode { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.LivePersonChat.MonitoringCode")]
        [AllowHtml]
        public string MonitoringCode { get; set; }
    }
}