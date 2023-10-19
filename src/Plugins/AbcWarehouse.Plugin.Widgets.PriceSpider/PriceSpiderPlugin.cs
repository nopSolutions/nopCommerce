using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Services.Cms;
using Nop.Core;

namespace AbcWarehouse.Plugin.Widgets.PriceSpider
{
    public class PriceSpiderPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "PriceSpider";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.CheckoutCompletedTop
            });
        }
    }
}
