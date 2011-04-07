using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.AustraliaPost
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.AustraliaPost.Configure",
                 "Plugins/ShippingAustraliaPost/Configure",
                 new { controller = "ShippingAustraliaPost", action = "Configure" },
                 new[] { "Nop.Plugin.Shipping.AustraliaPost.Controllers" }
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
