using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Weixin;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Senparc.Weixin.MP.CommonService.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //
            endpointRouteBuilder.MapControllerRoute("WeixinJSSDK", $"WeixinJSSDK/",
                new { controller = "WeixinJSSDK", action = "Index" });
            //WeixinApi
            endpointRouteBuilder.MapControllerRoute("Weixin", $"Weixin/",
                new { controller = "Weixin", action = "Index" });
            //Oauth2
            endpointRouteBuilder.MapControllerRoute(NopWeixinDefaults.WeixinOauthCallbackControler, $"OAuth2/",
                new { controller = "OAuth2", action = "Index" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1; //set a value that is greater than the default one in Nop.Web to override routes
    }
}