//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface IExternalAuthenticationMethod : IPlugin
    {
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
    }
}
