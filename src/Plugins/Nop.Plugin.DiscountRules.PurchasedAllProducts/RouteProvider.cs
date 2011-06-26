using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.PurchasedAllProducts
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.PurchasedAllProducts.Configure",
                 "Plugins/DiscountRulesPurchasedAllProducts/Configure",
                 new { controller = "DiscountRulesPurchasedAllProducts", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.PurchasedAllProducts.Controllers" }
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
