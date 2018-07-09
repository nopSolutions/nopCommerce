using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        
        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.GoogleId")]
        public string GoogleId { get; set; }
        public bool GoogleId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.EnableEcommerce")]
        public bool EnableEcommerce { get; set; }
        public bool EnableEcommerce_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.UseJsToSendEcommerceInfo")]
        public bool UseJsToSendEcommerceInfo { get; set; }
        public bool UseJsToSendEcommerceInfo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.TrackingScript")]
        public string TrackingScript { get; set; }
        public bool TrackingScript_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.IncludingTax")]
        public bool IncludingTax { get; set; }
        public bool IncludingTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.GoogleAnalytics.IncludeCustomerId")]
        public bool IncludeCustomerId { get; set; }
        public bool IncludeCustomerId_OverrideForStore { get; set; }
    }
}