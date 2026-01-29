using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.Polls.Infrastructure;

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
        //poll vote (AJAX)
        endpointRouteBuilder.MapControllerRoute(name: PollsDefaults.Routes.PollVoteRouteName,
            pattern: $"poll/vote",
            defaults: new { controller = "Poll", action = "Vote" });

        endpointRouteBuilder.MapControllerRoute(name: PollsDefaults.Routes.ConfigurationRouteName,
            pattern: "Admin/Poll/Configure",
            defaults: new { controller = "PollAdmin", action = "Configure", area = AreaNames.ADMIN });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}