using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.SMS.Clickatell
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.SMS.Clickatell.Configure",
                 "Plugins/SMSClickatell/Configure",
                 new { controller = "SmsClickatell", action = "Configure" },
                 new[] { "Nop.Plugin.SMS.Clickatell.Controllers" }
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
