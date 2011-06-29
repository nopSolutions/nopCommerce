using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.Fedex
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.Fedex.Configure",
                 "Plugins/ShippingFedex/Configure",
                 new { controller = "ShippingFedex", action = "Configure" },
                 new[] { "Nop.Plugin.Shipping.Fedex.Controllers" }
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
