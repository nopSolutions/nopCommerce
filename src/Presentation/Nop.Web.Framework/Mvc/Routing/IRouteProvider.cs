using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Route provider
    /// </summary>
    public interface IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRoute">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder endpointRoute);

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        int Priority { get; }
    }
}
