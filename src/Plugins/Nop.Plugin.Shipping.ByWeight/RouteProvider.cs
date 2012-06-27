using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.ByWeight
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.ByWeight.Configure",
                 "Plugins/ShippingByWeight/Configure",
                 new { controller = "ShippingByWeight", action = "Configure" },
                 new[] { "Nop.Plugin.Shipping.ByWeight.Controllers" }
            );

            routes.MapRoute("Plugin.Shipping.ByWeight.AddShippingRate",
                 "Plugins/ShippingByWeight/AddShippingRate",
                 new { controller = "ShippingByWeight", action = "AddShippingRate" },
                 new[] { "Nop.Plugin.Shipping.ByWeight.Controllers" }
            );
            routes.MapRoute("Plugin.Shipping.ByWeight.SaveGeneralSettings",
                 "Plugins/ShippingByWeight/SaveGeneralSettings",
                 new { controller = "ShippingByWeight", action = "SaveGeneralSettings" },
                 new[] { "Nop.Plugin.Shipping.ByWeight.Controllers" }
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
