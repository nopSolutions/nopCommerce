#if NET451
using System.Web.Routing;
#endif

namespace Nop.Web.Framework.Mvc.Routes
{
    /// <summary>
    /// Route publisher
    /// </summary>
    public interface IRoutePublisher
    {
#if NET451
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        void RegisterRoutes(RouteCollection routes);
#endif
    }
}
