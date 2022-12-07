using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Iyzico.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //IPN
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Iyzico.PaymentConfirm", "Plugins/PaymentIyzico/PaymentConfirm",
                 new { controller = "PaymentIyzicoPC", action = "PaymentConfirm" });

            //Cancel
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Iyzico.CancelOrder", "Plugins/PaymentIyzico/CancelOrder",
                 new { controller = "PaymentIyzico", action = "CancelOrder" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1;
    }
}