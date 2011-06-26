using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Tax.StrikeIron
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Tax.StrikeIron.Configure",
                 "Plugins/TaxStrikeIron/Configure",
                 new { controller = "TaxStrikeIron", action = "Configure" },
                 new[] { "Nop.Plugin.Tax.StrikeIron.Controllers" }
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
