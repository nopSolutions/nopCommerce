using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Events;
using Nop.Services.Seo;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.News.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class RoutingEventConsumer : IConsumer<GenericRoutingEvent>
{
    #region Fields

    private readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public RoutingEventConsumer(IUrlRecordService urlRecordService)
    {
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle routing event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(GenericRoutingEvent eventMessage)
    {
        if (string.IsNullOrEmpty(eventMessage?.UrlRecord?.EntityName) ||
            !eventMessage.UrlRecord.EntityName.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        //if URL record is not active let's find the latest one
        var slug = eventMessage.UrlRecord.IsActive
            ? eventMessage.UrlRecord.Slug
            : await _urlRecordService.GetActiveSlugAsync(eventMessage.UrlRecord.EntityId, eventMessage.UrlRecord.EntityName, eventMessage.UrlRecord.LanguageId);

        if (string.IsNullOrEmpty(slug))
            return;

        eventMessage.RouteValues[NopRoutingDefaults.RouteValue.Controller] = "News";
        eventMessage.RouteValues[NopRoutingDefaults.RouteValue.Action] = "NewsItem";
        eventMessage.RouteValues[NopRoutingDefaults.RouteValue.SeName] = slug;
        eventMessage.RouteValues[NewsDefaults.Routes.NewsItemIdRouteValue] = eventMessage.UrlRecord.EntityId;
        eventMessage.StopProcessing = true;
    }

    #endregion
}