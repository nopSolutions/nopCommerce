using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.SMS.Verizon
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.SMS.Verizon.Configure",
                 "Plugins/SMSVerizon/Configure",
                 new { controller = "SmsVerizon", action = "Configure" },
                 new[] { "Nop.Plugin.SMS.Verizon.Controllers" }
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
