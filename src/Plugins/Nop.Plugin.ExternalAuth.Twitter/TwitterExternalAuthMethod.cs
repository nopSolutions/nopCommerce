using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Twitter
{
    /// <summary>
    /// Twitter externalAuth processor
    /// </summary>
    public class TwitterExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields
        private readonly TwitterExternalAuthSettings _twitterExternalAuthSettings;
        #endregion

        #region Ctor

        public TwitterExternalAuthMethod(TwitterExternalAuthSettings twitterExternalAuthSettings)
        {
            this._twitterExternalAuthSettings = twitterExternalAuthSettings;
        }

        #endregion

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
            controllerName = "ExternalAuthTwitter";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Twitter.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying plugin in public store
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPublicInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "ExternalAuthTwitter";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Twitter.Controllers" }, { "area", null } };
        }
        
        #endregion
        
    }
}
