using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.GoogleId")]
        [AllowHtml]
        public string GoogleId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.JavaScript")]
        [AllowHtml]
        public string JavaScript { get; set; }
    }
}