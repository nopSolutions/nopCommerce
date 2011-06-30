using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.AuthorizeNet
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.AuthorizeNet.Configure",
                 "Plugins/PaymentAuthorizeNet/Configure",
                 new { controller = "PaymentAuthorizeNet", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.AuthorizeNet.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.AuthorizeNet.PaymentInfo",
                 "Plugins/PaymentAuthorizeNet/PaymentInfo",
                 new { controller = "PaymentAuthorizeNet", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.AuthorizeNet.Controllers" }
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
