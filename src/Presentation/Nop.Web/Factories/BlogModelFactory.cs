using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly BlogSettings _blogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IBlogService _blogService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;

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
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings)
        {
            _blogSettings = blogSettings;
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _blogService = blogService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _pictureService = pictureService;
            _cacheManager = cacheManager;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare blog comment model
        /// </summary>
        /// <param name="blogComment">Blog comment entity</param>
        /// <returns>Blog comment model</returns>
        public virtual BlogCommentModel PrepareBlogPostCommentModel(BlogComment blogComment)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            var model = new BlogCommentModel
            {
                Id = blogComment.Id,
                CustomerId = blogComment.CustomerId,
                CustomerName = _customerService.FormatUsername(blogComment.Customer),
                CommentText = blogComment.CommentText,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(blogComment.CreatedOnUtc, DateTimeKind.Utc),
                AllowViewingProfiles = _customerSettings.AllowViewingProfiles && blogComment.Customer != null && !blogComment.Customer.IsGuest()
            };
            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                    _genericAttributeService.GetAttribute<int>(blogComment.Customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
            }

            return model;
        }

        /// <summary>
        /// Prepare blog post model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post entity</param>
        /// <param name="prepareComments">Whether to prepare blog comments</param>
        public virtual void PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool prepareComments)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            model.Id = blogPost.Id;
            model.MetaTitle = blogPost.MetaTitle;
            model.MetaDescription = blogPost.MetaDescription;
            model.MetaKeywords = blogPost.MetaKeywords;
            model.SeName = _urlRecordService.GetSeName(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = blogPost.Title;
            model.Body = blogPost.Body;
            model.BodyOverview = blogPost.BodyOverview;
            model.AllowComments = blogPost.AllowComments;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogPost.StartDateUtc ?? blogPost.CreatedOnUtc, DateTimeKind.Utc);
            model.Tags = _blogService.ParseTags(blogPost);
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage;

            //number of blog comments
            var storeId = _blogSettings.ShowBlogCommentsPerStore ? _storeContext.CurrentStore.Id : 0;
            var cacheKey = string.Format(NopModelCacheDefaults.BlogCommentsNumberKey, blogPost.Id, storeId, true);
            model.NumberOfComments = _cacheManager.Get(cacheKey, () => _blogService.GetBlogCommentsCount(blogPost, storeId, true));

            if (prepareComments)
            {
                var blogComments = blogPost.BlogComments.Where(comment => comment.IsApproved);
                if (_blogSettings.ShowBlogCommentsPerStore)
                    blogComments = blogComments.Where(comment => comment.StoreId == _storeContext.CurrentStore.Id);

                foreach (var bc in blogComments.OrderBy(comment => comment.CreatedOnUtc))
                {
                    var commentModel = PrepareBlogPostCommentModel(bc);
                    model.Comments.Add(commentModel);
                }
            }
        }

        /// <summary>
        /// Prepare blog post list model
        /// </summary>
        /// <param name="command">Blog paging filtering model</param>
        /// <returns>Blog post list model</returns>
        public virtual BlogPostListModel PrepareBlogPostListModel(BlogPagingFilteringModel command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new BlogPostListModel();
            model.PagingFilteringContext.Tag = command.Tag;
            model.PagingFilteringContext.Month = command.Month;
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _blogSettings.PostsPageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var dateFrom = command.GetFromMonth();
            var dateTo = command.GetToMonth();

            IPagedList<BlogPost> blogPosts;
            if (string.IsNullOrEmpty(command.Tag))
            {
                blogPosts = _blogService.GetAllBlogPosts(_storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id,
                    dateFrom, dateTo, command.PageNumber - 1, command.PageSize);
            }
            else
            {
                blogPosts = _blogService.GetAllBlogPostsByTag(_storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id,
                    command.Tag, command.PageNumber - 1, command.PageSize);
            }
            model.PagingFilteringContext.LoadPagedList(blogPosts);

            model.BlogPosts = blogPosts
                .Select(x =>
                {
                    var blogPostModel = new BlogPostModel();
                    PrepareBlogPostModel(blogPostModel, x, false);
                    return blogPostModel;
                })
                .ToList();

            return model;
        }

        /// <summary>
        /// Prepare blog post tag list model
        /// </summary>
        /// <returns>Blog post tag list model</returns>
        public virtual BlogPostTagListModel PrepareBlogPostTagListModel()
        {
            var cacheKey = string.Format(NopModelCacheDefaults.BlogTagsModelKey, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new BlogPostTagListModel();

                //get tags
                var tags = _blogService.GetAllBlogPostTags(_storeContext.CurrentStore.Id, _workContext.WorkingLanguage.Id)
                    .OrderByDescending(x => x.BlogPostCount)
                    .Take(_blogSettings.NumberOfTags)
                    .ToList();
                //sorting
                tags = tags.OrderBy(x => x.Name).ToList();

                foreach (var tag in tags)
                    model.Tags.Add(new BlogPostTagModel
                    {
                        Name = tag.Name,
                        BlogPostCount = tag.BlogPostCount
                    });
                return model;
            });

            return cachedModel;
        }

        /// <summary>
        /// Prepare blog post year models
        /// </summary>
        /// <returns>List of blog post year model</returns>
        public virtual List<BlogPostYearModel> PrepareBlogPostYearModel()
        {
            var cacheKey = string.Format(NopModelCacheDefaults.BlogMonthsModelKey, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new List<BlogPostYearModel>();

                var blogPosts = _blogService.GetAllBlogPosts(_storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id);
                if (blogPosts.Any())
                {
                    var months = new SortedDictionary<DateTime, int>();

                    var blogPost = blogPosts[blogPosts.Count - 1];
                    var first = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc;
                    while (DateTime.SpecifyKind(first, DateTimeKind.Utc) <= DateTime.UtcNow.AddMonths(1))
                    {
                        var list = _blogService.GetPostsByDate(blogPosts, new DateTime(first.Year, first.Month, 1),
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

        #endregion
    }
}