using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.MediaMigration.Infrastructure;
public class RouteProvider : IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(
            name: "MediaMigration.Configure",
            pattern: "Admin/MediaMigration/Configure",
            defaults: new { controller = "MediaMigration", action = "Configure", area = "Admin" }
        );
    }

    public int Priority => 0;
}

