using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Shipping.FixedOrByWeight
{
    /// <summary>
    /// Route provider
    /// </summary>
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Shipping.FixedOrByWeight.Configure",
                 "Plugins/FixedOrByWeight/Configure",
                 new { controller = "FixedOrByWeight", action = "Configure", }
            );

            routeBuilder.MapRoute("Plugin.Shipping.FixedOrByWeight.AddRateByWeighPopup",
                 "Plugins/FixedOrByWeight/AddRateByWeighPopup",
                 new { controller = "FixedOrByWeight", action = "AddRateByWeighPopup", area = "Admin" }
            );

            routeBuilder.MapRoute("Plugin.Shipping.FixedOrByWeight.EditRateByWeighPopup",
                 "Plugins/FixedOrByWeight/EditRateByWeighPopup",
                 new { controller = "FixedOrByWeight", action = "EditRateByWeighPopup", area = "Admin" }
            );
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
