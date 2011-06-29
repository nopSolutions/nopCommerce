using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.USPS
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.USPS.Configure",
                 "Plugins/ShippingUSPS/Configure",
                 new { controller = "ShippingUSPS", action = "Configure" },
                 new[] { "Nop.Plugin.Shipping.USPS.Controllers" }
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
