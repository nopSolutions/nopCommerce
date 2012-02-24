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

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.TrackingScript")]
        [AllowHtml]
        //tracking code
        public string TrackingScript { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.EcommerceScript")]
        [AllowHtml]
        public string EcommerceScript { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.EcommerceDetailScript")]
        [AllowHtml]
        public string EcommerceDetailScript { get; set; }

    }
}