using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Feed.Froogle
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Feed.Froogle.Configure",
                 "Plugins/FeedFroogle/Configure",
                 new { controller = "FeedFroogle", action = "Configure" },
                 new[] { "Nop.Plugin.Feed.Froogle.Controllers" }
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
