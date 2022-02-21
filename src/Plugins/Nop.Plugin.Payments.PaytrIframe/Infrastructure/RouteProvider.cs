using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.PaytrIframe.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority { get { return -1; } }

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //payment
            endpointRouteBuilder.MapControllerRoute("Plugin.Payment", "plugin/payment/{orderId}",
                new { controller = "PaymentPaytrIframe", action = "PaytrPluginPayment" });

            //ajax for category based
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.PaytrIframe.CategoryOptions", "Plugins/PaymentPaytrIframe/CategoryOptions",
                new { controller = "PaymentPaytrIframe", action = "CategoryOptions" });

            //callback
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.PaytrIframe.Callback", "Plugins/PaymentPaytrIframe/Callback",
                new { controller = "PaymentPaytrIframe", action = "Callback" });

            //cancel order
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.PaytrIframe.CancelOrder", "Plugins/PaymentPaytrIframe/CancelOrder",
                new { controller = "PaymentPaytrIframe", action = "CancelOrder" });
        }
    }
}
