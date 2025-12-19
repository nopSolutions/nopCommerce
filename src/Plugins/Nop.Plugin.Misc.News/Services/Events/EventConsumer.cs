using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Plugin.Misc.News.Admin.Models;
using Nop.Plugin.Misc.News.Domain;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.Routing;
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
    IConsumer<ModelPreparedEvent<BaseNopModel>>,
    IConsumer<MetaTagsGeneratingEvent>
{
    #region Fields

    private readonly ICommonModelFactory _commonModelFactory;
    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly NewsService _newsService;

    #endregion

    #region Ctor

    public EventConsumer(ICommonModelFactory commonModelFactory,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IStaticCacheManager staticCacheManager,
        NewsService newsService)
    {
        _commonModelFactory = commonModelFactory;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
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
        if (eventMessage.MessageTemplate == null || string.IsNullOrEmpty(eventMessage.MessageTemplate.Name))
            return Task.CompletedTask;

        if (eventMessage.MessageTemplate.Name.Equals(NewsDefaults.NewsCommentStoreOwnerNotification, StringComparison.InvariantCultureIgnoreCase))
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

        if (eventMessage?.Model is EntityPreviewModel entityPreviewModel)
        {
            if (entityPreviewModel.ModelType == typeof(NewsItemModel))
            {
                var newsItem = await _newsService.GetNewsByIdAsync(entityPreviewModel.Id);
                entityPreviewModel.PreviewModels = await _commonModelFactory.PrepareMultistorePreviewModelsAsync(newsItem);
            }
        }

        if (eventMessage?.Model is UrlRecordListModel urlRecordListModel)
        {
            var newsItemsModels = urlRecordListModel.Data
                .Where(urlModel => string.Equals(urlModel.EntityName, nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            foreach (var newsItemModel in newsItemsModels)
            {
                newsItemModel.DetailsUrl = _nopUrlHelper.RouteUrl(NewsDefaults.Routes.Admin.NewsItemEditRouteName, new { id = newsItemModel.EntityId });
            }
        }
    }

    #endregion

    #region Meta tags

    /// <summary>
    /// Handle meta tags generating event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(MetaTagsGeneratingEvent eventMessage)
    {
        if (!eventMessage.EntityTypeName.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase))
            return;

        var newsItem = await _newsService.GetNewsByIdAsync(eventMessage.EntityId);
        if (newsItem is null)
            return;

        var textRequiredLocale = "Plugins.Misc.News.ArtificialIntelligence.NewsItemFullRequired";
        var titleRequiredLocale = "Plugins.Misc.News.ArtificialIntelligence.NewsItemTitleRequired";
        (eventMessage.Title, eventMessage.Text) = await eventMessage
            .ValidateTitleAndTextAsync(textRequiredLocale, titleRequiredLocale, newsItem.Title, newsItem.Full);

        eventMessage.CurrentMetaTitle = newsItem.MetaTitle;
        eventMessage.CurrentMetaDescription = newsItem.MetaDescription;
        eventMessage.CurrentMetaKeywords = newsItem.MetaKeywords;
        eventMessage.StopProcessing = true;
    }

    #endregion

    #endregion
}