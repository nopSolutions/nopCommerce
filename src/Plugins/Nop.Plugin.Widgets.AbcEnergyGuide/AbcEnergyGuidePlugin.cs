using System.Collections.Generic;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Plugin.Misc.AbcCore.Infrastructure;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcEnergyGuide
{
    public class AbcEnergyGuidePlugin : BasePlugin, IWidgetPlugin
    {
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { CustomPublicWidgetZones.ProductDetailsDescriptionTabTop });
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsAbcEnergyGuide";
        }

        public bool HideInWidgetList => false;
    }
}