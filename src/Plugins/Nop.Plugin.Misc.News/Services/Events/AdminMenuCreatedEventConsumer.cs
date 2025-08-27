using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.News.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class AdminMenuCreatedEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    #region Fields

    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly IUrlHelperFactory _urlHelperFactory;

    #endregion

    #region Ctor

    public AdminMenuCreatedEventConsumer(IActionContextAccessor actionContextAccessor,
        ILocalizationService localizationService,
        IUrlHelperFactory urlHelperFactory)
    {
        _actionContextAccessor = actionContextAccessor;
        _localizationService = localizationService;
        _urlHelperFactory = urlHelperFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle admin menu created event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        eventMessage.RootMenuItem.InsertAfter("Message templates", new AdminMenuItem
        {
            SystemName = NewsDefaults.NewsItemsMenuSystemName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems"),
            IconClass = "far fa-dot-circle",
            Url = urlHelper.RouteUrl(NewsDefaults.Routes.Admin.NewsItemsRouteName),
            PermissionNames = new List<string> { NewsDefaults.Permissions.NEWS_VIEW }
        });

        eventMessage.RootMenuItem.InsertAfter(NewsDefaults.NewsItemsMenuSystemName, new AdminMenuItem
        {
            SystemName = NewsDefaults.NewsCommentsMenuSystemName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments"),
            IconClass = "far fa-dot-circle",
            Url = urlHelper.RouteUrl(NewsDefaults.Routes.Admin.NewsCommentsRouteName),
            PermissionNames = new List<string> { NewsDefaults.Permissions.NEWS_COMMENTS_VIEW }
        });
    }

    #endregion
}