using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.NivoSlider
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Widgets.NivoSlider.Configure",
                 "Plugins/WidgetsNivoSlider/Configure",
                 new { controller = "WidgetsNivoSlider", action = "Configure" },
                 new[] { "Nop.Plugin.Widgets.NivoSlider.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.NivoSlider.PublicInfo",
                 "Plugins/WidgetsNivoSlider/PublicInfo",
                 new { controller = "WidgetsNivoSlider", action = "PublicInfo" },
                 new[] { "Nop.Plugin.Widgets.NivoSlider.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
