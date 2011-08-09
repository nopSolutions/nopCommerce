using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.LivePersonChat
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Widgets.LivePersonChat.Configure",
                 "Plugins/WidgetsLivePersonChat/Configure",
                 new { controller = "WidgetsLivePersonChat", action = "Configure" },
                 new[] { "Nop.Plugin.Widgets.LivePersonChat.Controllers" }
            );

            routes.MapRoute("Plugin.Widgets.LivePersonChat.PublicInfo",
                 "Plugins/WidgetsLivePersonChat/PublicInfo",
                 new { controller = "WidgetsLivePersonChat", action = "PublicInfo" },
                 new[] { "Nop.Plugin.Widgets.LivePersonChat.Controllers" }
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
