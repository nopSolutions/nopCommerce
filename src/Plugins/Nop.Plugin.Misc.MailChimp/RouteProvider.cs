using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.MailChimp
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Misc.MailChimp.Configure", "Plugins/MiscMailChimp/Configure",
                            new { controller = "Settings", action = "Index" },
                            new[] { "Nop.Plugin.Misc.MailChimp.Controllers" }
                );

            routes.MapRoute("Plugin.Misc.MailChimp.WebHook", "Plugins/MiscMailChimp/WebHook/{webHookKey}",
                new { controller = "WebHooks", action = "index" },
                new[] { "Nop.Plugin.Misc.MailChimp.Controllers" });
        }

        public int Priority
        {
            get { return 0; }
        }

    }
}