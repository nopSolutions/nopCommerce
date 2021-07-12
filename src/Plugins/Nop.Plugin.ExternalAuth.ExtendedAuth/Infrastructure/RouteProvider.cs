using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            // call back url routing for facebook            
            endpointRouteBuilder.MapControllerRoute(AuthenticationDefaults.FacebookCallbackPath, AuthenticationDefaults.FacebookCallbackPath,
                new { controller = "Authentication", action = "LoginCallback" });

            // call back url routing for linkedIn
            endpointRouteBuilder.MapControllerRoute(AuthenticationDefaults.LinkedInCallbackPath, AuthenticationDefaults.LinkedInCallbackPath,
               new { controller = "Authentication", action = "LoginCallback" });

            // call back url routing for microsoft            
            endpointRouteBuilder.MapControllerRoute(AuthenticationDefaults.MicrosoftCallbackPath, AuthenticationDefaults.MicrosoftCallbackPath,
               new { controller = "Authentication", action = "LoginCallback" });

            // call back url routing for Twitter            
            endpointRouteBuilder.MapControllerRoute(AuthenticationDefaults.TwitterCallbackPath, AuthenticationDefaults.TwitterCallbackPath,
               new { controller = "Authentication", action = "LoginCallback" });

            // call back url routing for Google            
            endpointRouteBuilder.MapControllerRoute(AuthenticationDefaults.GoogleCallbackPath, AuthenticationDefaults.GoogleCallbackPath,
               new { controller = "Authentication", action = "LoginCallback" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return -1; }
        }
    }
}
