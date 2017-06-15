using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.DiscountRules.CustomerRoles
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.DiscountRules.CustomerRoles.Configure",
                 "Plugins/DiscountRulesCustomerRoles/Configure",
                 new { controller = "DiscountRulesCustomerRoles", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.CustomerRoles.Controllers" }
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