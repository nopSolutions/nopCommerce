using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Web.Infrastructure.Cache;

namespace Nop.Plugin.Misc.News.Services.Events;

/// <summary>
/// Represents the plugin event consumer
/// </summary>
public class EventConsumer : IConsumer<CustomerPermanentlyDeleted>,
    IConsumer<EntityInsertedEvent<NewsItem>>,
    IConsumer<EntityUpdatedEvent<NewsItem>>,
    IConsumer<EntityDeletedEvent<NewsItem>>,
    IConsumer<EntityUpdatedEvent<Setting>>,
    IConsumer<AdditionalTokensAddedEvent>
{
    #region Fields

    private readonly IStaticCacheManager _staticCacheManager;
    private readonly NewsService _newsService;

    #endregion

    #region Ctor

    public EventConsumer(
        IStaticCacheManager staticCacheManager,
        NewsService newsService)
    {
        _staticCacheManager = staticCacheManager;
        _newsService = newsService;
    }

    #endregion

    #region Methods

    #region GDPR

    /// <summary>
    /// Handle customer permanently deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerPermanentlyDeleted eventMessage)
    {
        //news comments
        var newsComments = await _newsService.GetAllCommentsAsync(customerId: eventMessage.CustomerId);
        await _newsService.DeleteNewsCommentsAsync(newsComments);
    }


    #endregion

    #region News items

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<NewsItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
    }

    #endregion

    #region Setting

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
    {
        //clear models which depend on settings
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
    }

    #endregion

    #region Message templates

    public Task HandleEventAsync(AdditionalTokensAddedEvent eventMessage)
    {
        eventMessage.AddTokens("%NewsComment.NewsTitle%");

        return Task.CompletedTask;
    }

    #endregion

    #endregion
}
