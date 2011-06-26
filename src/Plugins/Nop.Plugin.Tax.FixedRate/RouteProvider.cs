using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Tax.FixedRate
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Tax.FixedRate.Configure",
                 "Plugins/TaxFixedRate/Configure",
                 new { controller = "TaxFixedRate", action = "Configure" },
                 new[] { "Nop.Plugin.Tax.FixedRate.Controllers" }
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
