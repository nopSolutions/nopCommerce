using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.FixedOrByWeight
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.FixedOrByWeight.Configure",
                 "Plugins/FixedOrByWeight/Configure",
                 new { controller = "FixedOrByWeight", action = "Configure", },
                 new[] { "Nop.Plugin.Shipping.FixedOrByWeight.Controllers" }
            );

            routes.MapRoute("Plugin.Shipping.FixedOrByWeight.AddRateByWeighPopup",
                 "Plugins/FixedOrByWeight/AddRateByWeighPopup",
                 new { controller = "FixedOrByWeight", action = "AddRateByWeighPopup" },
                 new[] { "Nop.Plugin.Shipping.FixedOrByWeight.Controllers" }
            );

            routes.MapRoute("Plugin.Shipping.FixedOrByWeight.EditRateByWeighPopup",
                 "Plugins/FixedOrByWeight/EditRateByWeighPopup",
                 new { controller = "FixedOrByWeight", action = "EditRateByWeighPopup" },
                 new[] { "Nop.Plugin.Shipping.FixedOrByWeight.Controllers" }
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
