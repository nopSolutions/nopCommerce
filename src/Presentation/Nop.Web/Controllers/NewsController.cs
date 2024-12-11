using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Rss;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.News;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class NewsController : BasePublicController
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsModelFactory _newsModelFactory;
    protected readonly INewsService _newsService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly NewsSettings _newsSettings;

    #endregion

    #region Ctor

    public NewsController(CaptchaSettings captchaSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INewsModelFactory newsModelFactory,
        INewsService newsService,
        INopUrlHelper nopUrlHelper,
        IPermissionService permissionService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        NewsSettings newsSettings)
    {
        _captchaSettings = captchaSettings;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _newsModelFactory = newsModelFactory;
        _newsService = newsService;
        _nopUrlHelper = nopUrlHelper;
        _permissionService = permissionService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _newsSettings = newsSettings;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> List(NewsPagingFilteringModel command)
    {
        if (!_newsSettings.Enabled)
            return RedirectToRoute("Homepage");

        var model = await _newsModelFactory.PrepareNewsItemListModelAsync(command);
        return View(model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> ListRss(int languageId)
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
            var newsUrl = await _nopUrlHelper.RouteGenericUrlAsync<NewsItem>(new { SeName = seName }, _webHelper.GetCurrentRequestProtocol());
            items.Add(new RssItem(n.Title, n.Short, new Uri(newsUrl), $"urn:store:{store.Id}:news:blog:{n.Id}", n.CreatedOnUtc));
        }
        feed.Items = items;
        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    public virtual async Task<IActionResult> NewsItem(int newsItemId)
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
        var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL) && await _permissionService.AuthorizeAsync(StandardPermission.ContentManagement.NEWS_VIEW);
        if (notAvailable && !hasAdminAccess)
            return InvokeHttp404();

        var model = new NewsItemModel();
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

        //display "edit" (manage) link
        if (hasAdminAccess)
            DisplayEditLink(Url.Action("NewsItemEdit", "News", new { id = newsItem.Id, area = AreaNames.ADMIN }));

        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
    {
        if (!_newsSettings.Enabled)
            return RedirectToRoute("Homepage");

        var newsItem = await _newsService.GetNewsByIdAsync(newsItemId);
        if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
            return RedirectToRoute("Homepage");

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("News.Comments.OnlyRegisteredUsersLeaveComments"));
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
                await _workflowMessageService.SendNewsCommentStoreOwnerNotificationMessageAsync(comment, _localizationSettings.DefaultAdminLanguageId);

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.AddNewsComment",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddNewsComment"), comment);

            //raise event
            if (comment.IsApproved)
                await _eventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

            //The text boxes should be cleared after a comment has been posted
            //That' why we reload the page
            TempData["nop.news.addcomment.result"] = comment.IsApproved
                ? await _localizationService.GetResourceAsync("News.Comments.SuccessfullyAdded")
                : await _localizationService.GetResourceAsync("News.Comments.SeeAfterApproving");

            var seName = await _urlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);
            var newsUrl = await _nopUrlHelper.RouteGenericUrlAsync<NewsItem>(new { SeName = seName });
            return LocalRedirect(newsUrl);
        }

        //If we got this far, something failed, redisplay form
        RouteData.Values["action"] = "NewsItem";
        model = await _newsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
        return View(model);
    }

    #endregion
}