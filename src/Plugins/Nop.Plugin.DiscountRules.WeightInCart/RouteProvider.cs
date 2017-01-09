using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.DiscountRules.WeightInCart
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.WeightInCart.Configure",
                 "Plugins/DiscountRulesWeightInCart/Configure",
                 new { controller = "DiscountRulesWeightInCart", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.WeightInCart.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.WeightInCart.ProductAddPopup",
                 "Plugins/DiscountRulesWeightInCart/ProductAddPopup",
                 new { controller = "DiscountRulesWeightInCart", action = "ProductAddPopup" },
                 new[] { "Nop.Plugin.DiscountRules.WeightInCart.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.WeightInCart.ProductAddPopupList",
                 "Plugins/DiscountRulesWeightInCart/ProductAddPopupList",
                 new { controller = "DiscountRulesWeightInCart", action = "ProductAddPopupList" },
                 new[] { "Nop.Plugin.DiscountRules.WeightInCart.Controllers" }
            );
            routes.MapRoute("Plugin.DiscountRules.WeightInCart.LoadProductFriendlyNames",
                 "Plugins/DiscountRulesWeightInCart/LoadProductFriendlyNames",
                 new { controller = "DiscountRulesWeightInCart", action = "LoadProductFriendlyNames" },
                 new[] { "Nop.Plugin.DiscountRules.WeightInCart.Controllers" }
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
