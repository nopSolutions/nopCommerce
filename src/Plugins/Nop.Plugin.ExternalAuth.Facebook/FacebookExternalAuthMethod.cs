using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Facebook
{
    /// <summary>
    /// Facebook externalAuth processor
    /// </summary>
    public class FacebookExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields
        private readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
        #endregion

        #region Ctor

        public FacebookExternalAuthMethod(FacebookExternalAuthSettings facebookExternalAuthSettings)
        {
            this._facebookExternalAuthSettings = facebookExternalAuthSettings;
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
            controllerName = "ExternalAuthFacebook";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Facebook.Controllers" }, { "area", null } };
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
            controllerName = "ExternalAuthFacebook";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.Facebook.Controllers" }, { "area", null } };
        }
        
        #endregion
        
    }
}
