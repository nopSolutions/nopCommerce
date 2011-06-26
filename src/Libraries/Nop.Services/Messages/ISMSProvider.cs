using System.Web.Routing;
using Nop.Core.Plugins;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Provides an interface for SMS providers
    /// </summary>
    public partial interface ISmsProvider : IPlugin
    {
        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        bool SendSms(string text);

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);

    }
}
