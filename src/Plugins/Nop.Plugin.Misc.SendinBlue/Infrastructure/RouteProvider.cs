using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.SendinBlue.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRoute">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapControllerRoute(SendinBlueDefaults.ImportContactsRoute, "Plugins/SendinBlue/ImportContacts",
                new { controller = "SendinBlue", action = "ImportContacts" });

            endpointRoute.MapControllerRoute(SendinBlueDefaults.UnsubscribeContactRoute, "Plugins/SendinBlue/UnsubscribeWebHook",
                new { controller = "SendinBlue", action = "UnsubscribeWebHook" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}