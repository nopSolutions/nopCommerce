using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Plugin.Misc.News.Admin.Factories;
using Nop.Plugin.Misc.News.Admin.Models;
using Nop.Plugin.Misc.News.Domain;
using Nop.Plugin.Misc.News.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.News.Admin.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
[SaveSelectedTab]
public class NewsAdminController : BasePluginController
{
    #region Fields

    private readonly ICustomerActivityService _customerActivityService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreService _storeService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly NewsModelFactory _newsModelFactory;
    private readonly NewsService _newsService;

    #endregion

    #region Ctor

    public NewsAdminController(ICustomerActivityService customerActivityService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        IUrlRecordService urlRecordService,
        NewsModelFactory newsModelFactory,
        NewsService newsService)
    {
        _customerActivityService = customerActivityService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _urlRecordService = urlRecordService;
        _newsModelFactory = newsModelFactory;
        _newsService = newsService;
    }

    #endregion

    #region Utilities

    protected async Task SaveStoreMappingsAsync(NewsItem newsItem, NewsItemModel model)
    {
        newsItem.LimitedToStores = model.SelectedStoreIds.Any();
        await _newsService.UpdateNewsAsync(newsItem);

        var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(newsItem);
        var allStores = await _storeService.GetAllStoresAsync();
        foreach (var store in allStores)
        {
            if (model.SelectedStoreIds.Contains(store.Id))
            {
                //new store
                if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                    await _storeMappingService.InsertStoreMappingAsync(newsItem, store.Id);
            }
            else
            {
                //remove store
                var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                if (storeMappingToDelete != null)
                    await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
            }
        }
    }

    #endregion

    #region Methods        

    #region Configure

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure()
    {
        var model = await _newsModelFactory.PrepareNewsConfigurationModelAsync();

        return View("~/Plugins/Misc.News/Admin/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid) //if we got this far, something failed, redisplay form
        {
            model = await _newsModelFactory.PrepareNewsConfigurationModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/Misc.News/Admin/Views/Configure.cshtml", model);
        }

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>(storeScope);
        newsSettings = model.ToSettings(newsSettings);

        //we do not clear cache after each setting update.
        //this behavior can increase performance because cached settings will not be cleared 
        //and loaded from database after each update
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.NotifyAboutNewNewsComments, model.NotifyAboutNewNewsComments_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.ShowNewsOnMainPage, model.ShowNewsOnMainPage_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.MainPageNewsCount, model.MainPageNewsCount_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.NewsArchivePageSize, model.NewsArchivePageSize_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.ShowHeaderRssUrl, model.ShowHeaderRssUrl_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.NewsCommentsMustBeApproved, model.NewsCommentsMustBeApproved_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.SitemapIncludeNews, model.SitemapIncludeNews_OverrideForStore, storeScope, false);
        //await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.DisplayNewsFooterItem, model.DisplayNewsFooterItem_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.ShowCaptchaOnNewsCommentPage, model.ShowCaptchaOnNewsCommentPage_OverrideForStore, storeScope, false);

        await _settingService.SaveSettingAsync(newsSettings, x => x.ShowNewsCommentsPerStore, clearCache: false);

        //now clear settings cache
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

        return RedirectToAction(nameof(Configure));
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> ShowCaptcha(bool showCaptchaOnNewsCommentPage)
    {
        if (!showCaptchaOnNewsCommentPage)
            return Json(new { Result = string.Empty });

        var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>();
        if (captchaSettings.Enabled)
            return Json(new { Result = string.Empty });

        var url = Url.Action("GeneralCommon", "Setting");
        var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Misc.News.Configuration.ShowCaptchaOnNewsCommentPage.Warning"), url);
        return Json(new { Result = warning });
    }

    #endregion

    #region News items

    public IActionResult Index()
    {
        return RedirectToAction(nameof(NewsItems));
    }

    [CheckPermission(NewsDefaults.Permissions.NEWS_VIEW)]
    public async Task<IActionResult> NewsItems(int? filterByNewsItemId)
    {
        //prepare model
        var model = await _newsModelFactory.PrepareNewsContentModelAsync(new NewsContentModel(), filterByNewsItemId);

        return View("~/Plugins/Misc.News/Admin/Views/NewsItems.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_VIEW)]
    public async Task<IActionResult> List(NewsItemSearchModel searchModel)
    {
        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(NewsDefaults.Permissions.NEWS_MANAGE)]
    public async Task<IActionResult> NewsItemCreate()
    {
        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemModelAsync(new NewsItemModel(), null);

        return View("~/Plugins/Misc.News/Admin/Views/NewsItemCreate.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(NewsDefaults.Permissions.NEWS_MANAGE)]
    public async Task<IActionResult> NewsItemCreate(NewsItemModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var newsItem = model.ToEntity<NewsItem>();
            newsItem.CreatedOnUtc = DateTime.UtcNow;
            await _newsService.InsertNewsAsync(newsItem);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewNews",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewNews"), newsItem.Id), newsItem);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
            await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

            //Stores
            await SaveStoreMappingsAsync(newsItem, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems.Added"));

            if (!continueEditing)
                return RedirectToAction(nameof(NewsItems));

            return RedirectToAction(nameof(NewsItemEdit), new { id = newsItem.Id });
        }

        //prepare model
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(NewsDefaults.Permissions.NEWS_VIEW)]
    public async Task<IActionResult> NewsItemEdit(int id)
    {
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(id);
        if (newsItem == null)
            return RedirectToAction(nameof(NewsItems));

        //prepare model
        var model = await _newsModelFactory.PrepareNewsItemModelAsync(null, newsItem);

        return View("~/Plugins/Misc.News/Admin/Views/NewsItemEdit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(NewsDefaults.Permissions.NEWS_MANAGE)]
    public async Task<IActionResult> NewsItemEdit(NewsItemModel model, bool continueEditing)
    {
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(model.Id);
        if (newsItem == null)
            return RedirectToAction(nameof(NewsItems));

        if (ModelState.IsValid)
        {
            newsItem = model.ToEntity(newsItem);
            await _newsService.UpdateNewsAsync(newsItem);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditNews",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNews"), newsItem.Id), newsItem);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
            await _urlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

            //stores
            await SaveStoreMappingsAsync(newsItem, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems.Updated"));

            if (!continueEditing)
                return RedirectToAction(nameof(NewsItems));

            return RedirectToAction(nameof(NewsItemEdit), new { id = newsItem.Id });
        }

        //prepare model
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

        //if we got this far, something failed, redisplay form
        return View("~/Plugins/Misc.News/Admin/Views/NewsItemEdit.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_MANAGE)]
    public async Task<IActionResult> Delete(int id)
    {
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(id);
        if (newsItem == null)
            return RedirectToAction(nameof(NewsItems));

        await _newsService.DeleteNewsAsync(newsItem);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteNews",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.News.NewsItems.Deleted"));

        return RedirectToAction(nameof(NewsItems));
    }

    #endregion

    #region Comments

    [CheckPermission(NewsDefaults.Permissions.NEWS_COMMENTS_VIEW)]
    public async Task<IActionResult> NewsComments(int? filterByNewsItemId)
    {
        //try to get a news item with the specified id
        var newsItem = await _newsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
        if (newsItem == null && filterByNewsItemId.HasValue)
            return RedirectToAction(nameof(NewsComments));

        //prepare model
        var model = await _newsModelFactory.PrepareNewsCommentSearchModelAsync(new NewsCommentSearchModel(), newsItem);

        return View("~/Plugins/Misc.News/Admin/Views/NewsComments.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_COMMENTS_VIEW)]
    public async Task<IActionResult> Comments(NewsCommentSearchModel searchModel)
    {
        //prepare model
        var model = await _newsModelFactory.PrepareNewsCommentListModelAsync(searchModel, searchModel.NewsItemId);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE)]
    public async Task<IActionResult> CommentUpdate(NewsCommentModel model)
    {
        //try to get a news comment with the specified id
        var comment = await _newsService.GetNewsCommentByIdAsync(model.Id)
            ?? throw new ArgumentException("No comment found with the specified id");

        var previousIsApproved = comment.IsApproved;

        //fill entity from model
        comment = model.ToEntity(comment);

        await _newsService.UpdateNewsCommentAsync(comment);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditNewsComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment"), comment.Id), comment);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE)]
    public async Task<IActionResult> CommentDelete(int id)
    {
        //try to get a news comment with the specified id
        var comment = await _newsService.GetNewsCommentByIdAsync(id)
            ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

        await _newsService.DeleteNewsCommentAsync(comment);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteNewsComment",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), comment.Id), comment);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE)]
    public async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var comments = await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray());

        await _newsService.DeleteNewsCommentsAsync(comments);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteNewsComment");
        await _customerActivityService.InsertActivitiesAsync("DeleteNewsComment", comments, newsComment => string.Format(activityLogFormat, newsComment.Id));

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE)]
    public async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter not approved comments
        var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved).ToList();

        foreach (var newsComment in newsComments)
        {
            newsComment.IsApproved = true;

            await _newsService.UpdateNewsCommentAsync(newsComment);
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment");
        await _customerActivityService.InsertActivitiesAsync("EditNewsComment", newsComments, newsComment => string.Format(activityLogFormat, newsComment.Id));

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(NewsDefaults.Permissions.NEWS_COMMENTS_MANAGE)]
    public async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter approved comments
        var newsComments = (await _newsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved).ToList();

        foreach (var newsComment in newsComments)
        {
            newsComment.IsApproved = false;

            await _newsService.UpdateNewsCommentAsync(newsComment);
        }

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.EditNewsComment");
        await _customerActivityService.InsertActivitiesAsync("EditNewsComment", newsComments, newsComment => string.Format(activityLogFormat, newsComment.Id));

        return Json(new { Result = true });
    }

    #endregion

    #endregion
}