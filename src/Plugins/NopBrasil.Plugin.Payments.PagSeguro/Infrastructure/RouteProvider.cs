using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace NopBrasil.Plugin.Payments.PagSeguro.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //Notification
            routeBuilder.MapRoute("Plugin.Payments.PagSeguro.Notification", "Plugins/PaymentPagSeguro/Notification",
                 new { controller = "NotificationPagSeguro", action = "PushNotification" });

            routeBuilder.MapRoute("Plugin.Payments.PagSeguro.GetNotification", "Plugins/PaymentPagSeguro/GetNotification",
                 new { controller = "NotificationPagSeguro", action = "GetPushNotification" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;
    }
}
