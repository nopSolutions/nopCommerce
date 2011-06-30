using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.PayInStore
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.PayInStore.Configure",
                 "Plugins/PaymentPayInStore/Configure",
                 new { controller = "PaymentPayInStore", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.PayInStore.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.PayInStore.PaymentInfo",
                 "Plugins/PaymentPayInStore/PaymentInfo",
                 new { controller = "PaymentPayInStore", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.PayInStore.Controllers" }
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
