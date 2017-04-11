//Contributor:  Nicholas Mayne

#if NET451
using System.Web.Routing;
#endif
using Nop.Core.Plugins;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Provides an interface for creating external authentication methods
    /// </summary>
    public partial interface IExternalAuthenticationMethod : IPlugin
    {
#if NET451
        /// <summary>
        /// Gets a route for plugin configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);
        

        /// <summary>
        /// Gets a route for displaying plugin in public store
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);
#endif
    }
}
