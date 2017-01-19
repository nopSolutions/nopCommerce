using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Services.Events;
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
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.News;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
    public partial class NewsController : BasePublicController
    {
        #region Fields

        private readonly INewsModelFactory _newsModelFactory;
        private readonly INewsService _newsService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IEventPublisher _eventPublisher;

        private readonly NewsSettings _newsSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Ctor

        public NewsController(INewsModelFactory newsModelFactory,
            INewsService newsService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IEventPublisher eventPublisher,
            NewsSettings newsSettings,
            LocalizationSettings localizationSettings, 
            CaptchaSettings captchaSettings)
        {
            this._newsModelFactory = newsModelFactory;
            this._newsService = newsService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._workflowMessageService = workflowMessageService;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._eventPublisher = eventPublisher;

            this._newsSettings = newsSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
        }

        #endregion
        
        #region Methods

        public ActionResult HomePageNews()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
                return Content("");

            var model = _newsModelFactory.PrepareHomePageNewsItemsModel();
            return PartialView(model);
        }

        public ActionResult List(NewsPagingFilteringModel command)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");

            var model = _newsModelFactory.PrepareNewsItemListModel(command);
            return View(model);
        }

        public ActionResult ListRss(int languageId)
        {
            var feed = new SyndicationFeed(
                string.Format("{0}: News", _storeContext.CurrentStore.GetLocalized(x => x.Name)),
                "News",
                new Uri(_webHelper.GetStoreLocation(false)),
                string.Format("urn:store:{0}:news", _storeContext.CurrentStore.Id),
                DateTime.UtcNow);

            if (!_newsSettings.Enabled)
                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

            var items = new List<SyndicationItem>();
            var newsItems = _newsService.GetAllNews(languageId, _storeContext.CurrentStore.Id);
            foreach (var n in newsItems)
            {
                string newsUrl = Url.RouteUrl("NewsItem", new { SeName = n.GetSeName(n.LanguageId, ensureTwoPublishedLanguages: false) }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");
                items.Add(new SyndicationItem(n.Title, n.Short, new Uri(newsUrl), String.Format("urn:store:{0}:news:blog:{1}", _storeContext.CurrentStore.Id, n.Id), n.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        public ActionResult NewsItem(int newsItemId)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");

            var newsItem = _newsService.GetNewsById(newsItemId);
            if (newsItem == null ||
                !newsItem.Published ||
                (newsItem.StartDateUtc.HasValue && newsItem.StartDateUtc.Value >= DateTime.UtcNow) ||
                (newsItem.EndDateUtc.HasValue && newsItem.EndDateUtc.Value <= DateTime.UtcNow) ||
                //Store mapping
                !_storeMappingService.Authorize(newsItem))
                return RedirectToRoute("HomePage");

            var model = new NewsItemModel();
            model = _newsModelFactory.PrepareNewsItemModel(model, newsItem, true);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                DisplayEditLink(Url.Action("Edit", "News", new { id = newsItem.Id, area = "Admin" }));

            return View(model);
        }

        [HttpPost, ActionName("NewsItem")]
        [PublicAntiForgery]
        [FormValueRequired("add-comment")]
        [CaptchaValidator]
        public ActionResult NewsCommentAdd(int newsItemId, NewsItemModel model, bool captchaValid)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");

            var newsItem = _newsService.GetNewsById(newsItemId);
            if (newsItem == null || !newsItem.Published || !newsItem.AllowComments)
                return RedirectToRoute("HomePage");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (_workContext.CurrentCustomer.IsGuest() && !_newsSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", _localizationService.GetResource("News.Comments.OnlyRegisteredUsersLeaveComments"));
            }

            if (ModelState.IsValid)
            {
                var comment = new NewsComment
                {
                    NewsItemId = newsItem.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    CommentTitle = model.AddNewComment.CommentTitle,
                    CommentText = model.AddNewComment.CommentText,
                    IsApproved = !_newsSettings.NewsCommentsMustBeApproved,
                    StoreId = _storeContext.CurrentStore.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                newsItem.NewsComments.Add(comment);
                _newsService.UpdateNews(newsItem);

                //notify a store owner;
                if (_newsSettings.NotifyAboutNewNewsComments)
                    _workflowMessageService.SendNewsCommentNotificationMessage(comment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddNewsComment", _localizationService.GetResource("ActivityLog.PublicStore.AddNewsComment"));

                //raise event
                if (comment.IsApproved)
                    _eventPublisher.Publish(new NewsCommentApprovedEvent(comment));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["nop.news.addcomment.result"] = comment.IsApproved 
                    ? _localizationService.GetResource("News.Comments.SuccessfullyAdded")
                    : _localizationService.GetResource("News.Comments.SeeAfterApproving");

                return RedirectToRoute("NewsItem", new { SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
            }


            //If we got this far, something failed, redisplay form
            model = _newsModelFactory.PrepareNewsItemModel(model, newsItem, true);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult RssHeaderLink()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
                return Content("");

            string link = string.Format("<link href=\"{0}\" rel=\"alternate\" type=\"{1}\" title=\"{2}: News\" />",
                Url.RouteUrl("NewsRSS", new { languageId = _workContext.WorkingLanguage.Id }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http"), MimeTypes.ApplicationRssXml, _storeContext.CurrentStore.GetLocalized(x => x.Name));

            return Content(link);
        }

        #endregion
    }
}
