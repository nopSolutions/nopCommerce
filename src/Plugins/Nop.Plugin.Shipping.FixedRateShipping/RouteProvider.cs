using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.FixedRateShipping
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.FixedRate.Configure",
                 "Plugins/ShippingFixedRate/Configure",
                 new { controller = "ShippingFixedRate", action = "Configure" },
                 new[] { "Nop.Plugin.Shipping.FixedRate.Controllers" }
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
