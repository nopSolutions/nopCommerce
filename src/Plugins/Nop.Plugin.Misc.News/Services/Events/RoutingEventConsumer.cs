using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Http;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Events;
using Nop.Services.Localization;
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

    private readonly ILanguageService _languageService;
    private readonly IStoreContext _storeContext;
    private readonly IUrlRecordService _urlRecordService;
    private readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public RoutingEventConsumer(ILanguageService languageService,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        LocalizationSettings localizationSettings)
    {
        _languageService = languageService;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Transform route values to redirect the request
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="path">Path</param>
    /// <param name="permanent">Whether the redirect should be permanent</param>
    protected void InternalRedirect(HttpContext httpContext, RouteValueDictionary values, string path, bool permanent)
    {
        values[NopRoutingDefaults.RouteValue.Controller] = "Common";
        values[NopRoutingDefaults.RouteValue.Action] = "InternalRedirect";
        values[NopRoutingDefaults.RouteValue.Url] = $"{httpContext.Request.PathBase}{path}{httpContext.Request.QueryString}";
        values[NopRoutingDefaults.RouteValue.PermanentRedirect] = permanent;
        httpContext.Items[NopHttpDefaults.GenericRouteInternalRedirect] = true;
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
        var urlRecord = eventMessage?.UrlRecord;
        if (string.IsNullOrEmpty(urlRecord?.EntityName) || !eventMessage.UrlRecord.EntityName.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase))
            return;

        //if URL record is not active let's find the latest one
        var slug = urlRecord.IsActive
            ? urlRecord.Slug
            : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);

        if (string.IsNullOrEmpty(slug))
            return;

        //Ensure that the slug is the same for the current language, 
        //otherwise it can cause some issues when customers choose a new language but a slug stays the same
        if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && eventMessage.RouteValues.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var langValue))
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
            var language = languages
                .FirstOrDefault(lang => lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                ?? languages.FirstOrDefault();

            var slugLocalized = await _urlRecordService.GetSeNameAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id, true, false);
            if (!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //redirect to the page for current language
                InternalRedirect(eventMessage.HttpContext, eventMessage.RouteValues, $"/{language.UniqueSeoCode}/{slugLocalized}", false);
                return;
            }
        }

        eventMessage.RouteValues[NopRoutingDefaults.RouteValue.Controller] = "News";
        eventMessage.RouteValues[NopRoutingDefaults.RouteValue.Action] = "NewsItem";
        eventMessage.RouteValues[NopRoutingDefaults.RouteValue.SeName] = slug;
        eventMessage.RouteValues[NewsDefaults.Routes.NewsItemIdRouteValue] = urlRecord.EntityId;
        eventMessage.StopProcessing = true;
    }

    #endregion
}