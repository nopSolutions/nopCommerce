using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.InfigoProductProvider.Infrastructure;

public class RouteProvider : IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(InfigoProductProviderDefaults.ConfigurationRouteName,
            "Admin/InfigoProductProvider/Configure",
            new { controller = "InfigoProductProvider", action = "Configure" });
    }

    public int Priority => 0;
}