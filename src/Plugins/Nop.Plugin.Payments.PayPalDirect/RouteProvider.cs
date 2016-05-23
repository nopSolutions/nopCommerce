using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.PayPalDirect
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.PayPalDirect.Webhook",
                 "Plugins/PaymentPayPalDirect/Webhook",
                 new { controller = "PaymentPayPalDirect", action = "WebhookEventsHandler" },
                 new[] { "Nop.Plugin.Payments.PayPalDirect.Controllers" }
            );
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
