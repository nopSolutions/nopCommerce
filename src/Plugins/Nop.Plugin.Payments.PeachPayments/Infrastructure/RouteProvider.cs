using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.PeachPayments.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(PeachPaymentsDefaults.ConfigurationRouteName,
                "Admin/PeachPayments/Configure",
                new { controller = "PeachPayments", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(PeachPaymentsDefaults.ResultRouteName,
                "Plugins/PeachPayments/Result",
                new { controller = "PeachPaymentsWebhook", action = "Result" });
            endpointRouteBuilder.MapControllerRoute(PeachPaymentsDefaults.CallbackRouteName,
    "Plugins/PeachPayments/Callback",
    new { controller = "PeachPaymentsWebhook", action = "Callback" });
        }
    }
}
