using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Synchrony
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("ConfirmSynchrony",
                 "checkout/confirm/synchrony",
                 new { controller = "SynchronyPayment", action = "Confirm" }
            );

            endpointRouteBuilder.MapControllerRoute("Nop.Plugin.Payments.Synchrony.PaymentPostInfo",
                 "SynchronyPayment/PaymentPostInfo",
                 new { controller = "SynchronyPayment", action = "PaymentPostInfo" }
            );

            endpointRouteBuilder.MapControllerRoute("Nop.Plugin.Payments.Synchrony.PaymentPostInfoStatus",
                 "SynchronyPayment/PaymentPostInfoStatus",
                 new { controller = "SynchronyPayment", action = "PaymentPostInfoStatus" }
            );

            endpointRouteBuilder.MapControllerRoute("Nop.Plugin.Payments.Synchrony.SavePaymentInfo",
                 "SynchronyPayment/SavePaymentInfo",
                 new { controller = "SynchronyPayment", action = "SavePaymentInfo" }
            );

        }
    }
}
