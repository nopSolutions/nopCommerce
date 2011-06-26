using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.BillingCountry
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.BillingCountry.Configure",
                 "Plugins/DiscountRulesBillingCountry/Configure",
                 new { controller = "DiscountRulesBillingCountry", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.BillingCountry.Controllers" }
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
