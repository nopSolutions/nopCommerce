using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.DiscountRules.Store
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.Store.Configure",
                 "Plugins/DiscountRulesStore/Configure",
                 new { controller = "DiscountRulesStore", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.Store.Controllers" }
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
