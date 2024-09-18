using Nop.Services.Cms;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Infrastructure;

namespace AbcWarehouse.Plugin.Widgets.PercentOffAppliancesMessage
{
    public class PercentOffAppliancesMessageWidget : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "PercentOffAppliancesMessage";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { CustomPublicWidgetZones.ProductDetailsAfterPrice });
        }
    }
}
