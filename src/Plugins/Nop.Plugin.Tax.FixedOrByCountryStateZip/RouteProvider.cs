using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Tax.FixedOrByCountryStateZip.AddRateByCountryStateZip",
                 "Plugins/FixedOrByCountryStateZip/AddRateByCountryStateZip",
                 new { controller = "FixedOrByCountryStateZip", action = "AddRateByCountryStateZip" },
                 new[] { "Nop.Plugin.Tax.FixedOrByCountryStateZip.Controllers" }
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