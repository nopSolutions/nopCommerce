using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Common;

namespace Nop.Plugin.LiveChat.LivePerson
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class LivePersonProvider : BasePlugin, ILiveChatProvider
    {
        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "LiveChatLivePerson";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.LiveChat.LivePerson.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "LiveChatLivePerson";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.LiveChat.LivePerson.Controllers" }, { "area", null } };
        }
        
    }
}
