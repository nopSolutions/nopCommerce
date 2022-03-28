using Nop.Services.Cms;
using Nop.Services.Plugins;
using System.Collections.Generic;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.Misc.AbcCore.Infrastructure;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcPromoBanners
{
    public class AbcSynchronyPaymentsPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "AbcSynchronyPayments";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.ProductBoxAddinfoMiddle,
                CustomPublicWidgetZones.ProductDetailsAfterPrice
            });
        }
    }
}
