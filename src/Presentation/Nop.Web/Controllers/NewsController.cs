using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.News;

namespace Nop.Web.Controllers
{
    [NopHttpsRequirement(SslRequirement.No)]
    public partial class NewsController : BasePublicController
    {
        #region Fields

        private readonly INewsService _newsService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;

        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Constructors

        public NewsController(INewsService newsService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            IPictureService pictureService, 
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IWorkflowMessageService workflowMessageService,
            IWebHelper webHelper,
            ICacheManager cacheManager, 
            ICustomerActivityService customerActivityService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            MediaSettings mediaSettings, 
            NewsSettings newsSettings,
            LocalizationSettings localizationSettings, 
            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings)
        {
            this._newsService = newsService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._workflowMessageService = workflowMessageService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._customerActivityService = customerActivityService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;

            this._mediaSettings = mediaSettings;
            this._newsSettings = newsSettings;
            this._localizationSettings = localizationSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool prepareComments)
        {
            if (newsItem == null)
                throw new ArgumentNullException("newsItem");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = newsItem.Id;
            model.MetaTitle = newsItem.MetaTitle;
            model.MetaDescription = newsItem.MetaDescription;
            model.MetaKeywords = newsItem.MetaKeywords;
            model.SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = newsItem.Title;
            model.Short = newsItem.Short;
            model.Full = newsItem.Full;
            model.AllowComments = newsItem.AllowComments;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
            model.NumberOfComments = newsItem.CommentCount;
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage;
            if (prepareComments)
            {
                var newsComments = newsItem.NewsComments.OrderBy(pr => pr.CreatedOnUtc);
                foreach (var nc in newsComments)
                {
                    var commentModel = new NewsCommentModel
                    {
                        Id = nc.Id,
                        CustomerId = nc.CustomerId,
                        CustomerName = nc.Customer.FormatUserName(),
                        CommentTitle = nc.CommentTitle,
                        CommentText = nc.CommentText,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(nc.CreatedOnUtc, DateTimeKind.Utc),
                        AllowViewingProfiles = _customerSettings.AllowViewingProfiles && nc.Customer != null && !nc.Customer.IsGuest(),
                    };
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        commentModel.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                            nc.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                            _mediaSettings.AvatarPictureSize,
                            _customerSettings.DefaultAvatarEnabled,
                            defaultPictureType: PictureType.Avatar);
                    }
                    model.Comments.Add(commentModel);
                }
            }
        }

        #endregion

        #region Methods

        public ActionResult HomePageNews()
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowNewsOnMainPage)
                return Content("");

            var cacheKey = string.Format(ModelCacheEventConsumer.HOMEPAGE_NEWSMODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id, 0, _newsSettings.MainPageNewsCount);
                return new HomePageNewsItemsModel
                {
                    WorkingLanguageId = _workContext.WorkingLanguage.Id,
                    NewsItems = newsItems
                        .Select(x =>
                                    {
                                        var newsModel = new NewsItemModel();
                                        PrepareNewsItemModel(newsModel, x, false);
                                        return newsModel;
                                    })
                        .ToList()
                };
            });

            //"Comments" property of "NewsItemModel" object depends on the current customer.
            //Furthermore, we just don't need it for home page news. So let's reset it.
            //But first we need to clone the cached model (the updated one should not be cached)
            var model = (HomePageNewsItemsModel)cachedModel.Clone();
            foreach (var newsItemModel in model.NewsItems)
                newsItemModel.Comments.Clear();
            return PartialView(model);
        }

        public ActionResult List(NewsPagingFilteringModel command)
        {
            if (!_newsSettings.Enabled)
                return RedirectToRoute("HomePage");

            var model = new NewsItemListModel();
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _newsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var newsItems = _newsService.GetAllNews(_workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id,
                command.PageNumber - 1, command.PageSize);
            model.PagingFilteringContext.LoadPagedList(newsItems);

            model.NewsItems = newsItems
                .Select(x =>
                {
                    var newsModel = new NewsItemModel();
                    PrepareNewsItemModel(newsModel, x, false);
                    return newsModel;
                })
                .ToList();

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
                return new RssActionResult { Feed = feed };

            var items = new List<SyndicationItem>();
            var newsItems = _newsService.GetAllNews(languageId, _storeContext.CurrentStore.Id);
            foreach (var n in newsItems)
            {
                string newsUrl = Url.RouteUrl("NewsItem", new { SeName = n.GetSeName(n.LanguageId, ensureTwoPublishedLanguages: false) }, _webHelper.IsCurrentConnectionSecured() ? "https" : "http");
                items.Add(new SyndicationItem(n.Title, n.Short, new Uri(newsUrl), String.Format("urn:store:{0}:news:blog:{1}", _storeContext.CurrentStore.Id, n.Id), n.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult { Feed = feed };
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
            PrepareNewsItemModel(model, newsItem, true);

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
                    CreatedOnUtc = DateTime.UtcNow,
                };
                newsItem.NewsComments.Add(comment);
                //update totals
                newsItem.CommentCount = newsItem.NewsComments.Count;
                _newsService.UpdateNews(newsItem);


                //notify a store owner;
                if (_newsSettings.NotifyAboutNewNewsComments)
                    _workflowMessageService.SendNewsCommentNotificationMessage(comment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddNewsComment", _localizationService.GetResource("ActivityLog.PublicStore.AddNewsComment"));

                //The text boxes should be cleared after a comment has been posted
                //That' why we reload the page
                TempData["nop.news.addcomment.result"] = _localizationService.GetResource("News.Comments.SuccessfullyAdded");
                return RedirectToRoute("NewsItem", new { SeName = newsItem.GetSeName(newsItem.LanguageId, ensureTwoPublishedLanguages: false) });
            }


            //If we got this far, something failed, redisplay form
            PrepareNewsItemModel(model, newsItem, true);
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
