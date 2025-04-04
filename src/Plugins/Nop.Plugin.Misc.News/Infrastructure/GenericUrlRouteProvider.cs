using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.News.Infrastructure;

/// <summary>
/// Represents provider that provided generic routes
/// </summary>
public class GenericUrlRouteProvider : BaseRouteProvider, IRouteProvider
{
    #region Methods

    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Public.NewsItemRouteName,
            pattern: $"{GetLanguageRoutePattern()}/{{{NopRoutingDefaults.RouteValue.SeName}}}",
            defaults: new { controller = "News", action = "NewsItem" });
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    /// <remarks>
    /// it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
    /// </remarks>
    public int Priority => -1000000;

    #endregion
}
