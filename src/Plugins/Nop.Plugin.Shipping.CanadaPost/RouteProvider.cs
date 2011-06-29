using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.CanadaPost
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.CanadaPost.Configure",
                 "Plugins/ShippingCanadaPost/Configure",
                 new { controller = "ShippingCanadaPost", action = "Configure" },
                 new[] { "Nop.Plugin.Shipping.CanadaPost.Controllers" }
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
