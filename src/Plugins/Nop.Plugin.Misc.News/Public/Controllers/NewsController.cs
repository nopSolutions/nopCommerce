using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Rss;
using Nop.Plugin.Misc.News.Domain;
using Nop.Plugin.Misc.News.Public.Factories;
using Nop.Plugin.Misc.News.Public.Models;
using Nop.Plugin.Misc.News.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.News.Public.Controllers;

[AutoValidateAntiforgeryToken]
public class NewsController : BasePublicController
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly NewsModelFactory _newsModelFactory;
    protected readonly NewsService _newsService;
    protected readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public NewsController(CaptchaSettings captchaSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        NewsModelFactory newsModelFactory,
        NewsService newsService,
        NewsSettings newsSettings)
    {
        _captchaSettings = captchaSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
        _newsModelFactory = newsModelFactory;
        _newsService = newsService;
        _newsSettings = newsSettings;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> List(NewsPagingFilteringModel command)
    {
        if (!_newsSettings.Enabled)
            return RedirectToRoute("Homepage");

        var model = await _newsModelFactory.PrepareNewsItemListModelAsync(command);
        return View("~/Plugins/Misc.News/Public/Views/List.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> ListRss(int languageId)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var feed = new RssFeed(
            $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: News",
            "News",
            new Uri(_webHelper.GetStoreLocation()),
            DateTime.UtcNow);

        if (!_newsSettings.Enabled)
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

        var items = new List<RssItem>();
        var newsItems = await _newsService.GetAllNewsAsync(languageId, store.Id);
        foreach (var n in newsItems)
        {
            var seName = await _urlRecordService.GetSeNameAsync(n, n.LanguageId, ensureTwoPublishedLanguages: false);
            var newsUrl = Url.RouteUrl(NewsDefaults.Routes.Public.NewsItemRouteName, new { SeName = seName }, _webHelper.GetCurrentRequestProtocol());
            items.Add(new RssItem(n.Title, n.Short, new Uri(newsUrl), $"urn:store:{store.Id}:news:blog:{n.Id}", n.CreatedOnUtc));
        }
        feed.Items = items;
        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    public async Task<IActionResult> NewsItem(int newsItemId)
    {
        if (!_newsSettings.Enabled)
            return RedirectToRoute("Homepage");

        var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
        if (newsItem == null)
            return InvokeHttp404();

        var notAvailable =
            //published?
            !newsItem.Published ||
            //availability dates
            !_newsService.IsNewsAvailable(newsItem) ||
            //Store mapping
            !await _storeMappingService.AuthorizeAsync(newsItem);
        //Check whether the current user has a "Manage news" permission (usually a store owner)
        //We should allows him (her) to use "Preview" functionality
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(NewsDefaults.Permissions.NEWS_VIEW);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        var model = new NewsItemModel();
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.RouteUrl(NewsDefaults.Routes.Admin.NewsItemEditRouteName, new { id = newsItem.Id }));

        return View("~/Plugins/Misc.News/Public/Views/NewsItem.cshtml", model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public async Task<IActionResult> NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
    {
        if (!_newsSettings.Enabled)
            return RedirectToRoute("Homepage");

        var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
        if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
            return RedirectToRoute("Homepage");

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _newsSettings.ShowCaptchaOnNewsCommentPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments.OnlyRegisteredUsersLeaveComments"));
        }

        if (ModelState.IsValid)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            var comment = new NewsComment
            {
                NewsItemId = newsItem.Id,
                CustomerId = customer.Id,
                CommentTitle = model.AddNewComment.CommentTitle,
                CommentText = model.AddNewComment.CommentText,
                IsApproved = !_newsSettings.NewsCommentsMustBeApproved,
                StoreId = store.Id,
                CreatedOnUtc = DateTime.UtcNow,
            };

            await _newsService.InsertNewsCommentAsync(comment);

            //notify a store owner;
            if (_newsSettings.NotifyAboutNewNewsComments)
                await _newsService.SendNewsCommentStoreOwnerNotificationMessageAsync(comment, _localizationSettings.DefaultAdminLanguageId);

            //activity log
            await _customerActivityService.InsertActivityAsync(NewsDefaults.ActivityLogTypeSystemNames.PublicStoreAddNewsComment,
                await _localizationService.GetResourceAsync("Plugins.Misc.News.ActivityLog.PublicStore.AddNewsComment"), comment);

            //The text boxes should be cleared after a comment has been posted
            //That' why we reload the page
            TempData["nop.news.addcomment.result"] = comment.IsApproved
                ? await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments.SuccessfullyAdded")
                : await _localizationService.GetResourceAsync("Plugins.Misc.News.Comments.SeeAfterApproving");

            var seName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);

            var newsUrl = Url.RouteUrl(NewsDefaults.Routes.Public.NewsItemRouteName, new { SeName = seName });
            return LocalRedirect(newsUrl);
        }

        //If we got this far, something failed, redisplay form
        RouteData.Values["action"] = "NewsItem";
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
        return View("~/Plugins/Misc.News/Public/Views/NewsItem.cshtml", model);
    }

    #endregion
}