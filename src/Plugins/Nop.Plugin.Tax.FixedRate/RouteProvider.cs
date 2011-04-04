using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
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
                 "Plugins/FixedTaxRate/Configure",
                 new { controller = "Config", action = "Configure" },
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
