using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Mvc.Routes
{
    /// <summary>
    /// Represents route provider
    /// </summary>
    public interface IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        void RegisterRoutes(IRouteBuilder routeBuilder);

        /// <summary>
        /// Gets a priority of route provider (more the better)
        /// </summary>
        int Priority { get; }
    }
}
