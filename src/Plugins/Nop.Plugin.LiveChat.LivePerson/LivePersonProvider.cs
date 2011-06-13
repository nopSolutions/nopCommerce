using System;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Payments;

namespace Nop.Plugin.LiveChat.LivePerson
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class LivePersonProvider : BasePlugin, ILiveChatProvider
    {
        #region Methods
        
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

        #endregion

        #region Properies

        /// <summary>
        /// Gets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get { return "LivePerson"; }
        }

        /// <summary>
        /// Gets the system name
        /// </summary>
        public override string SystemName
        {
            get { return "LiveChat.LivePerson"; }
        }

        /// <summary>
        /// Gets the author
        /// </summary>
        public override string Author
        {
            get { return "nopCommerce team"; }
        }

        /// <summary>
        /// Gets the version
        /// </summary>
        public override string Version
        {
            get { return "1.00"; }
        }

        #endregion
        
    }
}
