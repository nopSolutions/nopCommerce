using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Tax.Avalara.Infrastructure
{
    /// <summary>
    /// Represents route provider of Avalara tax provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //override some of default routes in Admin area
            routeBuilder.MapRoute("Plugin.Tax.Avalara.Tax.Categories", "Admin/Tax/Categories",
                new { controller = "OverriddenTax", action = "Categories", area = AreaNames.Admin });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1; //set a value that is greater than the default one in Nop.Web to override routes
    }
}