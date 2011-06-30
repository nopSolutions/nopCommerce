using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.CheckMoneyOrder
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.CheckMoneyOrder.Configure",
                 "Plugins/PaymentCheckMoneyOrder/Configure",
                 new { controller = "PaymentCheckMoneyOrder", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.CheckMoneyOrder.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.CheckMoneyOrder.PaymentInfo",
                 "Plugins/PaymentCheckMoneyOrder/PaymentInfo",
                 new { controller = "PaymentCheckMoneyOrder", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.CheckMoneyOrder.Controllers" }
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
