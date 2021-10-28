using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.News;
using Nop.Services.Seo;
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

        protected CaptchaSettings CaptchaSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected INewsService NewsService { get; }
        protected IPictureService PictureService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }
        protected NewsSettings NewsSettings { get; }

        #endregion

        #region Ctor

        public NewsModelFactory(CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            INewsService newsService,
            IPictureService pictureService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            NewsSettings newsSettings)
        {
            CaptchaSettings = captchaSettings;
            CustomerSettings = customerSettings;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            GenericAttributeService = genericAttributeService;
            NewsService = newsService;
            PictureService = pictureService;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
            NewsSettings = newsSettings;
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
        public virtual async Task<NewsItemModel> PrepareNewsItemModelAsync(NewsItemModel model, NewsItem newsItem, bool prepareComments)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (newsItem == null)
                throw new ArgumentNullException(nameof(newsItem));

            model.Id = newsItem.Id;
            model.MetaTitle = newsItem.MetaTitle;
            model.MetaDescription = newsItem.MetaDescription;
            model.MetaKeywords = newsItem.MetaKeywords;
            model.SeName = await UrlRecordService.GetSeNameAsync(newsItem, newsItem.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = newsItem.Title;
            model.Short = newsItem.Short;
            model.Full = newsItem.Full;
            model.AllowComments = newsItem.AllowComments;

            model.PreventNotRegisteredUsersToLeaveComments =
                await CustomerService.IsGuestAsync(await WorkContext.GetCurrentCustomerAsync()) &&
                !NewsSettings.AllowNotRegisteredUsersToLeaveComments;

            model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(newsItem.StartDateUtc ?? newsItem.CreatedOnUtc, DateTimeKind.Utc);
            model.AddNewComment.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnNewsCommentPage;

            //number of news comments
            var store = await StoreContext.GetCurrentStoreAsync();
            var storeId = NewsSettings.ShowNewsCommentsPerStore ? store.Id : 0;

            model.NumberOfComments = await NewsService.GetNewsCommentsCountAsync(newsItem, storeId, true);

            if (prepareComments)
            {
                var newsComments = await NewsService.GetAllCommentsAsync(
                    newsItemId: newsItem.Id,
                    approved: true,
                    storeId: NewsSettings.ShowNewsCommentsPerStore ? store.Id : 0);

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
        public virtual async Task<HomepageNewsItemsModel> PrepareHomepageNewsItemsModelAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var language = await WorkContext.GetWorkingLanguageAsync();
            var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepageNewsModelKey, language, store);
            var cachedModel = await StaticCacheManager.GetAsync(cacheKey, async () =>
            {
                var newsItems = await NewsService.GetAllNewsAsync(language.Id, store.Id, 0, NewsSettings.MainPageNewsCount);

                return new HomepageNewsItemsModel
                {
                    WorkingLanguageId = language.Id,
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
        public virtual async Task<NewsItemListModel> PrepareNewsItemListModelAsync(NewsPagingFilteringModel command)
        {
            if (command.PageSize <= 0)
                command.PageSize = NewsSettings.NewsArchivePageSize;
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            var language = await WorkContext.GetWorkingLanguageAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var newsItems = await NewsService.GetAllNewsAsync(language.Id, store.Id, command.PageNumber - 1, command.PageSize);

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
        public virtual async Task<NewsCommentModel> PrepareNewsCommentModelAsync(NewsComment newsComment)
        {
            if (newsComment == null)
                throw new ArgumentNullException(nameof(newsComment));

            var customer = await CustomerService.GetCustomerByIdAsync(newsComment.CustomerId);

            var model = new NewsCommentModel
            {
                Id = newsComment.Id,
                CustomerId = newsComment.CustomerId,
                CustomerName = await CustomerService.FormatUsernameAsync(customer),
                CommentTitle = newsComment.CommentTitle,
                CommentText = newsComment.CommentText,
                CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(newsComment.CreatedOnUtc, DateTimeKind.Utc),
                AllowViewingProfiles = CustomerSettings.AllowViewingProfiles && newsComment.CustomerId != 0 && !await CustomerService.IsGuestAsync(customer),
            };

            if (CustomerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerAvatarUrl = await PictureService.GetPictureUrlAsync(
                    await GenericAttributeService.GetAttributeAsync<Customer, int>(newsComment.CustomerId, NopCustomerDefaults.AvatarPictureIdAttribute),
                    MediaSettings.AvatarPictureSize, CustomerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
            }

            return model;
        }

        #endregion
    }
}