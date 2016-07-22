using Nop.Web.Framework.Mvc.Routes;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.DiscountRules.HasOrderedXTimes
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.DiscountRules.HasOrderedXTimes.Configure",
                 "Plugins/DiscountRulesHasOrderedXTimes/Configure",
                 new { controller = "DiscountRulesHasOrderedXTimes", action = "Configure" },
                 new[] { "Nop.Plugin.DiscountRules.HasOrderedXTimes.Controllers" }
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
