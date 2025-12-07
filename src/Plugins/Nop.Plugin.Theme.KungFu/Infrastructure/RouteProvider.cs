using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Theme.KungFu.Infrastructure;

public class RouteProvider : IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: ThemeKungFuDefaults.ConfigurationRouteName,
            pattern: "Admin/ThemeKungFu/Configure",
            defaults: new { controller = "ThemeKungFu", action = "Configure", area = AreaNames.ADMIN });
    }

    public int Priority => 0;
}
