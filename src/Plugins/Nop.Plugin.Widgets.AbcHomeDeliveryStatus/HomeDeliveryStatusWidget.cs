using Nop.Services.Cms;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus
{
    public class HomeDeliveryStatusWidget : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsAbcHomeDeliveryStatus";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { "topic_page_after_body" });
        }
    }
}
