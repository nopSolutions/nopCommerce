using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.News;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class NewsController : BasePublicController
    {
        #region Fields

        protected CaptchaSettings CaptchaSettings { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INewsModelFactory NewsModelFactory { get; }
        protected INewsService NewsService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected NewsSettings NewsSettings { get; }

        #endregion

        #region Ctor

        public NewsController(CaptchaSettings captchaSettings,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INewsModelFactory newsModelFactory,
            INewsService newsService,
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
            CaptchaSettings = captchaSettings;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            NewsModelFactory = newsModelFactory;
            NewsService = newsService;
            PermissionService = permissionService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            UrlRecordService = urlRecordService;
            WebHelper = webHelper;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
            NewsSettings = newsSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List(NewsPagingFilteringModel command)
        {
            if (!NewsSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await NewsModelFactory.PrepareNewsItemListModelAsync(command);
            return View(model);
        }

        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> ListRss(int languageId)
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var feed = new RssFeed(
                $"{await LocalizationService.GetLocalizedAsync(store, x => x.Name)}: News",
                "News",
                new Uri(WebHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!NewsSettings.Enabled)
                return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();
            var newsItems = await NewsService.GetAllNewsAsync(languageId, store.Id);
            foreach (var n in newsItems)
            {
                var newsUrl = Url.RouteUrl("NewsItem", new { SeName = await UrlRecordService.GetSeNameAsync(n, n.LanguageId, ensureTwoPublishedLanguages: false) }, WebHelper.GetCurrentRequestProtocol());
                items.Add(new RssItem(n.Title, n.Short, new Uri(newsUrl), $"urn:store:{store.Id}:news:blog:{n.Id}", n.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult(feed, WebHelper.GetThisPageUrl(false));
        }

        public virtual async Task<IActionResult> NewsItem(int newsItemId)
        {
            if (!NewsSettings.Enabled)
                return RedirectToRoute("Homepage");

            var newsItem = await NewsService.GetNewsByIdAsync(newsItemId);
            if (newsItem == null)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !newsItem.Published ||
                //availability dates
                !NewsService.IsNewsAvailable(newsItem) ||
                //Store mapping
                !await StoreMappingService.AuthorizeAsync(newsItem);
            //Check whether the current user has a "Manage news" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await PermissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            var model = new NewsItemModel();
            model = await NewsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

            //display "edit" (manage) link
            if (hasAdminAccess)
                DisplayEditLink(Url.Action("NewsItemEdit", "News", new { id = newsItem.Id, area = AreaNames.Admin }));

            return View(model);
        }

        [HttpPost, ActionName("NewsItem")]        
        [FormValueRequired("add-comment")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
        {
            if (!NewsSettings.Enabled)
                return RedirectToRoute("Homepage");

            var newsItem = await NewsService.GetNewsByIdAsync(newsItemId);
            if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
                return RedirectToRoute("Homepage");

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnNewsCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsGuestAsync(customer) && !NewsSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("News.Comments.OnlyRegisteredUsersLeaveComments"));
            }

            if (ModelState.IsValid)
            {
                var store = await StoreContext.GetCurrentStoreAsync();

                var comment = new NewsComment
                {
                    NewsItemId = newsItem.Id,
                    CustomerId = customer.Id,
                    CommentTitle = model.AddNewComment.CommentTitle,
                    CommentText = model.AddNewComment.CommentText,
                    IsApproved = !NewsSettings.NewsCommentsMustBeApproved,
                    StoreId = store.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                };

                await NewsService.InsertNewsCommentAsync(comment);

                //notify a store owner;
                if (NewsSettings.NotifyAboutNewNewsComments)
                    await WorkflowMessageService.SendNewsCommentNotificationMessageAsync(comment, LocalizationSettings.DefaultAdminLanguageId);

                //activity log
                await CustomerActivityService.InsertActivityAsync("PublicStore.AddNewsComment",
                    await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.AddNewsComment"), comment);

                //raise event
                if (comment.IsApproved)
                    await EventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["nop.news.addcomment.result"] = comment.IsApproved
                    ? await LocalizationService.GetResourceAsync("News.Comments.SuccessfullyAdded")
                    : await LocalizationService.GetResourceAsync("News.Comments.SeeAfterApproving");

                return RedirectToRoute("NewsItem", new { SeName = await UrlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
            }

            //If we got this far, something failed, redisplay form
            model = await NewsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);
            return View(model);
        }

        #endregion
    }
}