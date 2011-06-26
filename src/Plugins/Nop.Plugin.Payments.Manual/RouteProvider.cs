using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.Manual
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.Manual.Configure",
                 "Plugins/PaymentManual/Configure",
                 new { controller = "PaymentManual", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.Manual.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Manual.PaymentInfo",
                 "Plugins/PaymentManual/PaymentInfo",
                 new { controller = "PaymentManual", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.Manual.Controllers" }
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
