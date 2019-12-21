using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Square.Infrastructure
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
            //add route for the access token callback
            endpointRoute.MapControllerRoute(SquarePaymentDefaults.AccessTokenRoute, "Plugins/PaymentSquare/AccessToken/",
                new { controller = "PaymentSquare", action = "AccessTokenCallback" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}