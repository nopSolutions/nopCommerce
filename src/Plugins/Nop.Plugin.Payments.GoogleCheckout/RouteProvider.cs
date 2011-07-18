using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.GoogleCheckout
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.GoogleCheckout.Configure",
                 "Plugins/PaymentGoogleCheckout/Configure",
                 new { controller = "PaymentGoogleCheckout", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.GoogleCheckout.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.GoogleCheckout.PaymentInfo",
                 "Plugins/PaymentGoogleCheckout/PaymentInfo",
                 new { controller = "PaymentGoogleCheckout", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.GoogleCheckout.Controllers" }
            );
            
            
            //Submit Google Checkout button
            routes.MapRoute("Plugin.Payments.GoogleCheckout.SubmitButton",
                 "Plugins/PaymentGoogleCheckout/SubmitButton",
                 new { controller = "PaymentGoogleCheckout", action = "SubmitButton" },
                 new[] { "Nop.Plugin.Payments.GoogleCheckout.Controllers" }
                 );

            //Notification
            routes.MapRoute("Plugin.Payments.GoogleCheckout.NotificationHandler",
                 "Plugins/PaymentGoogleCheckout/NotificationHandler",
                 new { controller = "PaymentGoogleCheckout", action = "NotificationHandler" },
                 new[] { "Nop.Plugin.Payments.GoogleCheckout.Controllers" }
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
