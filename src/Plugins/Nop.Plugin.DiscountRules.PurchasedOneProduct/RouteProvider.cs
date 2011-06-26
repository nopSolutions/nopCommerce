using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.PurchasedOneProduct
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.PurchasedOneProduct.Configure",
                 "Plugins/DiscountRulesPurchasedOneProduct/Configure",
                 new { controller = "DiscountRulesPurchasedOneProduct", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.PurchasedOneProduct.Controllers" }
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
