using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.Sendinblue.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(SendinblueDefaults.ImportContactsRoute, "Plugins/Sendinblue/ImportContacts",
                new { controller = "Sendinblue", action = "ImportContacts" });

            endpointRouteBuilder.MapControllerRoute(SendinblueDefaults.UnsubscribeContactRoute, "Plugins/Sendinblue/UnsubscribeWebHook",
                new { controller = "Sendinblue", action = "UnsubscribeWebHook" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}