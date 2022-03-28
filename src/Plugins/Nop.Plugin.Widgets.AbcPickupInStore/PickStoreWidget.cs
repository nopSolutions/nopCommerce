using Nop.Core;
using Nop.Plugin.Misc.AbcCore.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcPickupInStore
{
    public class PickStoreWidget : BasePlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;

        public PickStoreWidget(
            IWebHelper webHelper
        )
        {
            _webHelper = webHelper;
        }

        public bool HideInWidgetList => false;

        public static class LocaleKey
        {
            public const string PickupInStoreText = "Plugins.Widgets.AbcPickupInStore.Fields.PickupInStoreText";
            public const string GoogleMapsAPIKey = "Plugins.Widgets.AbcPickupInStore.Fields.GoogleMapsAPIKey";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            // productdetails_before_collateral = bottom of page

            return Task.FromResult<IList<string>>(new List<string> {
                CustomPublicWidgetZones.ProductDetailsBeforeAddToCart,
                "productdetails_before_tabs"
            });
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "AbcPickupInStore";
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AbcContactUs/Configure";
        }
    }
}
