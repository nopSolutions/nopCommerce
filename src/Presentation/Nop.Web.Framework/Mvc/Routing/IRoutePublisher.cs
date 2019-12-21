using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents route publisher
    /// </summary>
    public interface IRoutePublisher
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRoute">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder endpointRoute);
    }
}
