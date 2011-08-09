using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.GoogleAnalytics
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Widgets.GoogleAnalytics.Configure",
                 "Plugins/WidgetsGoogleAnalytics/Configure",
                 new { controller = "WidgetsGoogleAnalytics", action = "Configure" },
                 new[] { "Nop.Plugin.Widgets.GoogleAnalytics.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.GoogleAnalytics.PublicInfo",
                 "Plugins/WidgetsGoogleAnalytics/PublicInfo",
                 new { controller = "WidgetsGoogleAnalytics", action = "PublicInfo" },
                 new[] { "Nop.Plugin.Widgets.GoogleAnalytics.Controllers" }
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
