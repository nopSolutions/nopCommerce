﻿using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Plugin.Misc.News.Domain;
using Nop.Plugin.Misc.News.Public.Models;
using Nop.Plugin.Misc.News.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Misc.News.Public.Factories;

/// <summary>
/// Represents the news model factory
/// </summary>
public class NewsModelFactory
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IPictureService _pictureService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly NewsService _newsService;
    protected readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public NewsModelFactory(CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IGenericAttributeService genericAttributeService,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        NewsService newsService,
        NewsSettings newsSettings)
    {
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _genericAttributeService = genericAttributeService;
        _newsService = newsService;
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _newsSettings = newsSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the news item model
    /// </summary>
    /// <param name="model">News item model</param>
    /// <param name="newsItem">News item</param>
    /// <param name="prepareComments">Whether to prepare news comment models</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news item model
    /// </returns>
    public async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool prepareComments)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(newsItem);

        model.Id = newsItem.Id;
        model.MetaTitle = newsItem.MetaTitle;
        model.MetaDescription = newsItem.MetaDescription;
        model.MetaKeywords = newsItem.MetaKeywords;
        model.SeName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);
        model.Title = newsItem.Title;
        model.Short = newsItem.Short;
        model.Full = newsItem.Full;
        model.AllowComments = newsItem.AllowComments;

        model.PreventNotRegisteredUsersToLeaveComments =
            await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) &&
            !_newsSettings.AllowNotRegisteredUsersToLeaveComments;

        model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
        model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _newsSettings.ShowCaptchaOnNewsCommentPage;

        var store = await _storeContext.GetCurrentStoreAsync();
        var storeId = _newsSettings.ShowNewsCommentsPerStore ? store.Id : 0;

        if (prepareComments)
        {
            var newsComments = await _newsService.GetAllCommentsAsync(
                newsItemId: newsItem.Id,
                approved: true,
                storeId: _newsSettings.ShowNewsCommentsPerStore ? store.Id : 0);

            foreach (var nc in newsComments.OrderBy(comment => comment.CreatedOnUtc))
            {
                var commentModel = await PrepareNewsCommentModelAsync(nc);
                model.Comments.Add(commentModel);
            }
        }

        return model;
    }

    /// <summary>
    /// Prepare the home page news items model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the home page news items model
    /// </returns>
    public async Task<HomepageNewsItemsModel> PrepareHomepageNewsItemsModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NewsDefaults.HomepageNewsModelKey, language, store);
        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var newsItems = await _newsService.GetAllNewsAsync(language.Id, store.Id, 0, _newsSettings.MainPageNewsCount);

            return new HomepageNewsItemsModel
            {
                NewsItems = await newsItems.SelectAwait(async newsItem =>
                {
                    var newsModel = new NewsItemModel();
                    await PrepareNewsItemModelAsync(newsModel, newsItem, false);
                    return newsModel;
                }).ToListAsync()
            };
        });

        //"Comments" property of "NewsItemModel" object depends on the current customer.
        //Furthermore, we just don't need it for home page news. So let's reset it.
        //But first we need to clone the cached model (the updated one should not be cached)
        var model = cachedModel with { };
        foreach (var newsItemModel in model.NewsItems)
            newsItemModel.Comments.Clear();

        return model;
    }

    /// <summary>
    /// Prepare the news item list model
    /// </summary>
    /// <param name="command">News paging filtering model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news item list model
    /// </returns>
    public async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsPagingFilteringModel command)
    {
        if (command.PageSize <= 0)
            command.PageSize = _newsSettings.NewsArchivePageSize;
        if (command.PageNumber <= 0)
            command.PageNumber = 1;

        var language = await _workContext.GetWorkingLanguageAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var newsItems = await _newsService.GetAllNewsAsync(language.Id, store.Id, command.PageNumber - 1, command.PageSize);

        var model = new NewsItemListModel
        {
            WorkingLanguageId = language.Id,
            NewsItems = await newsItems.SelectAwait(async newsItem =>
            {
                var newsModel = new NewsItemModel();
                await PrepareNewsItemModelAsync(newsModel, newsItem, false);
                return newsModel;
            }).ToListAsync()
        };
        model.PagingFilteringContext.LoadPagedList(newsItems);

        return model;
    }

    /// <summary>
    /// Prepare the news comment model
    /// </summary>
    /// <param name="newsComment">News comment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the news comment model
    /// </returns>
    public async Task<NewsCommentModel> PrepareNewsCommentModelAsync(NewsComment newsComment)
    {
        ArgumentNullException.ThrowIfNull(newsComment);

        var customer = await _customerService.GetCustomerByIdAsync(newsComment.CustomerId);

        var model = new NewsCommentModel
        {
            Id = newsComment.Id,
            CustomerId = newsComment.CustomerId,
            CustomerName = await _customerService.FormatUsernameAsync(customer),
            CommentTitle = newsComment.CommentTitle,
            CommentText = newsComment.CommentText,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc),
            AllowViewingProfiles = _customerSettings.AllowViewingProfiles && newsComment.CustomerId != 0 && !await _customerService.IsGuestAsync(customer),
        };

        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            model.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<Customer, int>(newsComment.CustomerId, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
        }

        return model;
    }

    #endregion
}