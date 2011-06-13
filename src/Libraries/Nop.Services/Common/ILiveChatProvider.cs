using System;
using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Plugins;

namespace Nop.Services.Common
{
    /// <summary>
    /// Provides an interface for creating live chat providers gateways & methods
    /// </summary>
    public partial interface ILiveChatProvider : IPlugin
    {
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);

        /// <summary>
        /// Gets a route for getting information for a public store
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);
    }
}
