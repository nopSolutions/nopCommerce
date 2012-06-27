using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Tax.CountryStateZip
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Tax.CountryStateZip.Configure",
                 "Plugins/TaxCountryStateZip/Configure",
                 new { controller = "TaxCountryStateZip", action = "Configure" },
                 new[] { "Nop.Plugin.Tax.CountryStateZip.Controllers" }
            );

            routes.MapRoute("Plugin.Tax.CountryStateZip.AddTaxRate",
                 "Plugins/TaxCountryStateZip/AddTaxRate",
                 new { controller = "TaxCountryStateZip", action = "AddTaxRate" },
                 new[] { "Nop.Plugin.Tax.CountryStateZip.Controllers" }
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
