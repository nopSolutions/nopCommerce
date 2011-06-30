using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.CashOnDelivery
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.CashOnDelivery.Configure",
                 "Plugins/PaymentCashOnDelivery/Configure",
                 new { controller = "PaymentCashOnDelivery", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.CashOnDelivery.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.CashOnDelivery.PaymentInfo",
                 "Plugins/PaymentCashOnDelivery/PaymentInfo",
                 new { controller = "PaymentCashOnDelivery", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.CashOnDelivery.Controllers" }
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
