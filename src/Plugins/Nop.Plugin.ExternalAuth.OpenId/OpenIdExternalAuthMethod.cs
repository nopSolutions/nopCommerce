using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.OpenId
{
    /// <summary>
    /// OpenId externalAuth processor
    /// </summary>
    public class OpenIdExternalAuthMethod : BasePlugin, IExternalAuthenticationMethod
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
            //configuration is not required
            actionName = null;
            controllerName = null;
            routeValues = null;
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
            controllerName = "ExternalAuthOpenId";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.ExternalAuth.OpenId.Controllers" }, { "area", null } };
        }
        
        #endregion
        
    }
}
