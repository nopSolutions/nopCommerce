using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.PurchaseOrder
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.PurchaseOrder.Configure",
                 "Plugins/PaymentPurchaseOrder/Configure",
                 new { controller = "PaymentPurchaseOrder", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.PurchaseOrder.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.PurchaseOrder.PaymentInfo",
                 "Plugins/PaymentPurchaseOrder/PaymentInfo",
                 new { controller = "PaymentPurchaseOrder", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.PurchaseOrder.Controllers" }
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
