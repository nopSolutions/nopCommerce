using Microsoft.AspNetCore.Routing;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents route publisher
    /// </summary>
    public partial interface IRoutePublisher
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder);
    }
}
