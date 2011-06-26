using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.ShippingCountry
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.ShippingCountry.Configure",
                 "Plugins/DiscountRulesShippingCountry/Configure",
                 new { controller = "DiscountRulesShippingCountry", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.ShippingCountry.Controllers" }
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
