using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.ByWeight
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.ByWeight.SaveGeneralSettings",
                 "Plugins/ShippingByWeight/SaveGeneralSettings",
                 new { controller = "ShippingByWeight", action = "SaveGeneralSettings", },
                 new[] { "Nop.Plugin.Shipping.ByWeight.Controllers" }
            );

            routes.MapRoute("Plugin.Shipping.ByWeight.AddPopup",
                 "Plugins/ShippingByWeight/AddPopup",
                 new { controller = "ShippingByWeight", action = "AddPopup" },
                 new[] { "Nop.Plugin.Shipping.ByWeight.Controllers" }
            );
            routes.MapRoute("Plugin.Shipping.ByWeight.EditPopup",
                 "Plugins/ShippingByWeight/EditPopup",
                 new { controller = "ShippingByWeight", action = "EditPopup" },
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
