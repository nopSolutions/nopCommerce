using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.News;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the news model factory
    /// </summary>
    public partial class NewsModelFactory : INewsModelFactory
    {
        #region Fields

        private readonly INewsService _newsService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICacheManager _cacheManager;

        private readonly MediaSettings _mediaSettings;
        private readonly NewsSettings _newsSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Constructors

        public NewsModelFactory(INewsService newsService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            IPictureService pictureService, 
            IDateTimeHelper dateTimeHelper,
            ICacheManager cacheManager,
            MediaSettings mediaSettings, 
            NewsSettings newsSettings,
            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings)
        {
            this._newsService = newsService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._pictureService = pictureService;
            this._dateTimeHelper = dateTimeHelper;
            this._cacheManager = cacheManager;

            this._mediaSettings = mediaSettings;
            this._newsSettings = newsSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the news comment model
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <returns>News comment model</returns>
        public virtual NewsCommentModel PrepareNewsCommentModel(NewsComment newsComment)
        {
            if (newsComment == null)
                throw new ArgumentNullException("newsComment");

            var model = new NewsCommentModel
            {
                Id = newsComment.Id,
                CustomerId = newsComment.CustomerId,
                CustomerName = newsComment.Customer.FormatUserName(),
                CommentTitle = newsComment.CommentTitle,
                CommentText = newsComment.CommentText,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc),
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && newsComment.Customer != null && !newsComment.Customer.IsGuest(),
            };
            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                    newsComment.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                    _mediaSettings.AvatarPictureSize,
                    _customerSettings.DefaultAvatarEnabled,
                    defaultPictureType: PictureType.Avatar);
            }

            return model;
        }

        /// <summary>
        /// Prepare the news item model
        /// </summary>
        /// <param name="model">News item model</param>
        /// <param name="newsItem">News item</param>
        /// <param name="prepareComments">Whether to prepare news comment models</param>
        /// <returns>News item model</returns>
        public virtual NewsItemModel PrepareNewsItemModel(NewsItemModel model, NewsItem newsItem, bool prepareComments)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (newsItem == null)
                throw new ArgumentNullException("newsItem");

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
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnNewsCommentPage;

            //number of news comments
            var storeId = _newsSettings.ShowNewsCommentsPerStore ? _storeContext.CurrentStore.Id : 0;
            var cacheKey = string.Format(ModelCacheEventConsumer.NEWS_COMMENTS_NUMBER_KEY, newsItem.Id, storeId, true);
            model.NumberOfComments = _cacheManager.Get(cacheKey, () => _newsService.GetNewsCommentsCount(newsItem, storeId, true));

            if (prepareComments)
            {
                var newsComments = newsItem.NewsComments.Where(comment => comment.IsApproved);
                if (_newsSettings.ShowNewsCommentsPerStore)
                    newsComments = newsComments.Where(comment => comment.StoreId == _storeContext.CurrentStore.Id);

                foreach (var nc in newsComments.OrderBy(comment => comment.CreatedOnUtc))
                {
                    var commentModel = PrepareNewsCommentModel(nc);
                    model.Comments.Add(commentModel);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the home page news items model
        /// </summary>
        /// <returns>Home page news items model</returns>
        public virtual HomePageNewsItemsModel PrepareHomePageNewsItemsModel()
        {
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
            return model;
        }

        /// <summary>
        /// Prepare the news item list model
        /// </summary>
        /// <param name="command">News paging filtering model</param>
        /// <returns>News item list model</returns>
        public virtual NewsItemListModel PrepareNewsItemListModel(NewsPagingFilteringModel command)
        {
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

            return model;
        }

        #endregion
    }
}
