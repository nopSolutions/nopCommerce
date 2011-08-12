using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.ExternalAuth.Facebook
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.ExternalAuth.Facebook.Configure",
                 "Plugins/ExternalAuthFacebook/Configure",
                 new { controller = "ExternalAuthFacebook", action = "Configure" },
                 new[] { "Nop.Plugin.ExternalAuth.Facebook.Controllers" }
            );

            routes.MapRoute("Plugin.ExternalAuth.Facebook.PublicInfo",
                 "Plugins/ExternalAuthFacebook/PublicInfo",
                 new { controller = "ExternalAuthFacebook", action = "PublicInfo" },
                 new[] { "Nop.Plugin.ExternalAuth.Facebook.Controllers" }
            );

            routes.MapRoute("Plugin.ExternalAuth.Facebook.Login",
                 "Plugins/ExternalAuthFacebook/Login",
                 new { controller = "ExternalAuthFacebook", action = "Login" },
                 new[] { "Nop.Plugin.ExternalAuth.Facebook.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
