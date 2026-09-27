using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Payments.WorldpayHpp.Infrastructure;
/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : BaseRouteProvider, IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var lang = GetLanguageRoutePattern();

        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Configure",
     "Admin/WorldpayHpp/Configure",
     new { controller = "WorldpayHpp", action = "Configure", area = AreaNames.ADMIN });


        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Webhook",
         pattern: "WorldpayHpp/Webhook",
         defaults: new { controller = "WorldpayHppWebhook", action = "WebhookHandler" });


        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Success",
            "WorldpayHpp/Success",
            new { controller = "WorldpayHppPublic", action = "Success" });

        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Failure",
            "WorldpayHpp/Failure",
            new { controller = "WorldpayHppPublic", action = "Failure" });

        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Cancel",
            "WorldpayHpp/Cancel",
            new { controller = "WorldpayHppPublic", action = "Cancel" });


        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Error",
           "WorldpayHpp/Error",
           new { controller = "WorldpayHppPublic", action = "Error" });


        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Expiry",
           "WorldpayHpp/Expiry",
           new { controller = "WorldpayHppPublic", action = "Expiry" });

        endpointRouteBuilder.MapControllerRoute("WorldpayHpp.Pending",
         "WorldpayHpp/Pending",
         new { controller = "WorldpayHppPublic", action = "Pending" });

    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}