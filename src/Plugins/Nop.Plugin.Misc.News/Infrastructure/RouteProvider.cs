using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.News.Infrastructure;

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
        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Admin.ConfigurationRouteName,
            pattern: "Admin/News/Configure",
            defaults: new { controller = "NewsAdmin", action = "Configure", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Admin.NewsItemsRouteName,
            pattern: "Admin/News",
            defaults: new { controller = "NewsAdmin", action = "NewsItems", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Admin.NewsItemEditRouteName,
            pattern: "Admin/News/Edit/{id:min(0)?}",
            defaults: new { controller = "NewsAdmin", action = "NewsItemEdit", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Admin.NewsItemCreateRouteName,
            pattern: "Admin/News/Create",
            defaults: new { controller = "NewsAdmin", action = "NewsItemCreate", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Admin.NewsItemDeleteRouteName,
            pattern: "Admin/News/Delete/{id:min(0)}",
            defaults: new { controller = "NewsAdmin", action = "Delete", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Admin.NewsCommentsRouteName,
            pattern: "Admin/News/Comments/{filterByNewsItemId:min(0)?}",
            defaults: new { controller = "NewsAdmin", action = "NewsComments", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Public.NewsArchive,
            pattern: $"{GetLanguageRoutePattern()}/news",
            defaults: new { controller = "News", action = "List" });

        //RSS (file result)
        endpointRouteBuilder.MapControllerRoute(name: NewsDefaults.Routes.Public.NewsRSS,
            pattern: "news/rss/{languageId:min(0)}",
            defaults: new { controller = "News", action = "ListRss" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}