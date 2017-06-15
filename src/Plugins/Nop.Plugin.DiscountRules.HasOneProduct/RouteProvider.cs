using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.DiscountRules.HasOneProduct
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.DiscountRules.HasOneProduct.Configure",
                 "Plugins/DiscountRulesHasOneProduct/Configure",
                 new { controller = "DiscountRulesHasOneProduct", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.HasOneProduct.Controllers" }
            );
            routeBuilder.MapRoute("Plugin.DiscountRules.HasOneProduct.ProductAddPopup",
                 "Plugins/DiscountRulesHasOneProduct/ProductAddPopup",
                 new { controller = "DiscountRulesHasOneProduct", action = "ProductAddPopup" },
                 new[] { "Nop.Plugin.DiscountRules.HasOneProduct.Controllers" }
            );
            routeBuilder.MapRoute("Plugin.DiscountRules.HasOneProduct.ProductAddPopupList",
                 "Plugins/DiscountRulesHasOneProduct/ProductAddPopupList",
                 new { controller = "DiscountRulesHasOneProduct", action = "ProductAddPopupList" },
                 new[] { "Nop.Plugin.DiscountRules.HasOneProduct.Controllers" }
            );
            routeBuilder.MapRoute("Plugin.DiscountRules.HasOneProduct.LoadProductFriendlyNames",
                 "Plugins/DiscountRulesHasOneProduct/LoadProductFriendlyNames",
                 new { controller = "DiscountRulesHasOneProduct", action = "LoadProductFriendlyNames" },
                 new[] { "Nop.Plugin.DiscountRules.HasOneProduct.Controllers" }
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