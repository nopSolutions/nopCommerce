using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.LiveChat.LivePerson.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.LiveChat.LivePerson.ButtonCode")]
        [AllowHtml]
        public string ButtonCode { get; set; }

        [NopResourceDisplayName("Plugins.LiveChat.LivePerson.MonitoringCode")]
        [AllowHtml]
        public string MonitoringCode { get; set; }
    }
}