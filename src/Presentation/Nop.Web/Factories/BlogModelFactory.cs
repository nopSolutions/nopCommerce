using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Services.Blogs;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Blogs;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the blog model factory
    /// </summary>
    public partial class BlogModelFactory : IBlogModelFactory
    {
        #region Fields

        protected BlogSettings BlogSettings { get; }
        protected CaptchaSettings CaptchaSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected IBlogService BlogService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IPictureService PictureService { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreContext StoreContext { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }

        #endregion

        #region Ctor

        public BlogModelFactory(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            IBlogService blogService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            IPictureService pictureService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            BlogSettings = blogSettings;
            CaptchaSettings = captchaSettings;
            CustomerSettings = customerSettings;
            BlogService = blogService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            GenericAttributeService = genericAttributeService;
            PictureService = pictureService;
            StaticCacheManager = staticCacheManager;
            StoreContext = storeContext;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare blog post model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post entity</param>
        /// <param name="prepareComments">Whether to prepare blog comments</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool prepareComments)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            model.Id = blogPost.Id;
            model.MetaTitle = blogPost.MetaTitle;
            model.MetaDescription = blogPost.MetaDescription;
            model.MetaKeywords = blogPost.MetaKeywords;
            model.SeName = await UrlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = blogPost.Title;
            model.Body = blogPost.Body;
            model.BodyOverview = blogPost.BodyOverview;
            model.AllowComments = blogPost.AllowComments;

            model.PreventNotRegisteredUsersToLeaveComments =
                await CustomerService.IsGuestAsync(await WorkContext.GetCurrentCustomerAsync()) &&
                !BlogSettings.AllowNotRegisteredUsersToLeaveComments;

            model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(blogPost.StartDateUtc ?? blogPost.CreatedOnUtc, DateTimeKind.Utc);
            model.Tags = await BlogService.ParseTagsAsync(blogPost);
            model.AddNewComment.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnBlogCommentPage;

            //number of blog comments
            var store = await StoreContext.GetCurrentStoreAsync();
            var storeId = BlogSettings.ShowBlogCommentsPerStore ? store.Id : 0;

            model.NumberOfComments = await BlogService.GetBlogCommentsCountAsync(blogPost, storeId, true);

            if (prepareComments)
            {
                var blogComments = await BlogService.GetAllCommentsAsync(
                    blogPostId: blogPost.Id,
                    approved: true,
                    storeId: storeId);

                foreach (var bc in blogComments)
                {
                    var commentModel = await PrepareBlogPostCommentModelAsync(bc);
                    model.Comments.Add(commentModel);
                }
            }
        }

        /// <summary>
        /// Prepare blog post list model
        /// </summary>
        /// <param name="command">Blog paging filtering model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog post list model
        /// </returns>
        public virtual async Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPagingFilteringModel command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.PageSize <= 0)
                command.PageSize = BlogSettings.PostsPageSize;
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            var dateFrom = command.GetFromMonth();
            var dateTo = command.GetToMonth();

            var language = await WorkContext.GetWorkingLanguageAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var blogPosts = string.IsNullOrEmpty(command.Tag)
                ? await BlogService.GetAllBlogPostsAsync(store.Id, language.Id, dateFrom, dateTo, command.PageNumber - 1, command.PageSize)
                : await BlogService.GetAllBlogPostsByTagAsync(store.Id, language.Id, command.Tag, command.PageNumber - 1, command.PageSize);

            var model = new BlogPostListModel
            {
                PagingFilteringContext = { Tag = command.Tag, Month = command.Month },
                WorkingLanguageId = language.Id,
                BlogPosts = await blogPosts.SelectAwait(async blogPost =>
                {
                    var blogPostModel = new BlogPostModel();
                    await PrepareBlogPostModelAsync(blogPostModel, blogPost, false);
                    return blogPostModel;
                }).ToListAsync()
            };
            model.PagingFilteringContext.LoadPagedList(blogPosts);

            return model;
        }

        /// <summary>
        /// Prepare blog post tag list model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog post tag list model
        /// </returns>
        public virtual async Task<BlogPostTagListModel> PrepareBlogPostTagListModelAsync()
        {
            var model = new BlogPostTagListModel();
            var store = await StoreContext.GetCurrentStoreAsync();

            //get tags
            var tags = (await BlogService
                .GetAllBlogPostTagsAsync(store.Id, (await WorkContext.GetWorkingLanguageAsync()).Id))
                .OrderByDescending(x => x.BlogPostCount)
                .Take(BlogSettings.NumberOfTags);

            //sorting and setting into the model
            model.Tags.AddRange(tags.OrderBy(x => x.Name).Select(tag => new BlogPostTagModel
            {
                Name = tag.Name,
                BlogPostCount = tag.BlogPostCount
            }));

            return model;
        }

        /// <summary>
        /// Prepare blog post year models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of blog post year model
        /// </returns>
        public virtual async Task<List<BlogPostYearModel>> PrepareBlogPostYearModelAsync()
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
            var cacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.BlogMonthsModelKey, currentLanguage, store);
            var cachedModel = await StaticCacheManager.GetAsync(cacheKey, async () =>
            {
                var model = new List<BlogPostYearModel>();

                var blogPosts = await BlogService.GetAllBlogPostsAsync(store.Id,
                    currentLanguage.Id);
                if (blogPosts.Any())
                {
                    var months = new SortedDictionary<DateTime, int>();

                    var blogPost = blogPosts[blogPosts.Count - 1];
                    var first = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc;
                    while (DateTime.SpecifyKind(first, DateTimeKind.Utc) <= DateTime.UtcNow.AddMonths(1))
                    {
                        var list = await BlogService.GetPostsByDateAsync(blogPosts, new DateTime(first.Year, first.Month, 1),
                            new DateTime(first.Year, first.Month, 1).AddMonths(1).AddSeconds(-1));
                        if (list.Any())
                        {
                            var date = new DateTime(first.Year, first.Month, 1);
                            months.Add(date, list.Count);
                        }

                        first = first.AddMonths(1);
                    }

                    var current = 0;
                    foreach (var kvp in months)
                    {
                        var date = kvp.Key;
                        var blogPostCount = kvp.Value;
                        if (current == 0)
                            current = date.Year;

                        if (date.Year > current || !model.Any())
                        {
                            var yearModel = new BlogPostYearModel
                            {
                                Year = date.Year
                            };
                            model.Insert(0, yearModel);
                        }

                        model.First().Months.Insert(0, new BlogPostMonthModel
                        {
                            Month = date.Month,
                            BlogPostCount = blogPostCount
                        });

                        current = date.Year;
                    }
                }

                return model;
            });

            return cachedModel;
        }
        
        /// <summary>
        /// Prepare blog comment model
        /// </summary>
        /// <param name="blogComment">Blog comment entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog comment model
        /// </returns>
        public virtual async Task<BlogCommentModel> PrepareBlogPostCommentModelAsync(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            var customer = await CustomerService.GetCustomerByIdAsync(blogComment.CustomerId);

            var model = new BlogCommentModel
            {
                Id = blogComment.Id,
                CustomerId = blogComment.CustomerId,
                CustomerName = await CustomerService.FormatUsernameAsync(customer),
                CommentText = blogComment.CommentText,
                CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc),
                AllowViewingProfiles = CustomerSettings.AllowViewingProfiles && customer != null && !await CustomerService.IsGuestAsync(customer)
            };

            if (CustomerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerAvatarUrl = await PictureService.GetPictureUrlAsync(
                    await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    MediaSettings.AvatarPictureSize, CustomerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
            }

            return model;
        }

        #endregion
    }
}