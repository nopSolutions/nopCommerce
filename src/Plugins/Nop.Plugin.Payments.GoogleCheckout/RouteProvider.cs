using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.GoogleCheckout
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
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
