using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.UPS
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.UPS.Configure",
                 "Plugins/ShippingUPS/Configure",
                 new { controller = "ShippingUPS", action = "Configure" },
                 new[] { "Nop.Plugin.Shipping.UPS.Controllers" }
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
