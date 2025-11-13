using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
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
    IConsumer<AdditionalTokensAddedEvent>,
    IConsumer<ModelPreparedEvent<BaseNopModel>>
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly NewsService _newsService;

    #endregion

    #region Ctor

    public EventConsumer(ILocalizationService localizationService,
        IStaticCacheManager staticCacheManager,
        NewsService newsService)
    {
        _localizationService = localizationService;
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

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
    }

    /// <summary>
    /// Handle entity updated event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
    }

    /// <summary>
    /// Handle entity deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<NewsItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey);
    }

    #endregion

    #region Setting

    /// <summary>
    /// Handle entity updated event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
    {
        //clear models which depend on settings
        await _staticCacheManager.RemoveByPrefixAsync(NewsDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
    }

    #endregion

    #region Message templates

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(AdditionalTokensAddedEvent eventMessage)
    {
        eventMessage.AddTokens("%NewsComment.NewsTitle%");

        return Task.CompletedTask;
    }

    #endregion

    #region Admin area

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage?.Model is MenuItemModel menuItemModel)
        {
            menuItemModel.AvailableStandardRoutes.Add(new()
            {
                Text = await _localizationService.GetResourceAsync("Plugins.Misc.News.NewsArchive"),
                Value = NewsDefaults.Routes.Public.NewsArchive
            });
        }
    }

    #endregion

    #endregion
}