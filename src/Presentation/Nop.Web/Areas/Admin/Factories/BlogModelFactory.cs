using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;
using Nop.Services.Blogs;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the blog model factory implementation
    /// </summary>
    public partial class BlogModelFactory : IBlogModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IBlogService _blogService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public BlogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IBlogService blogService,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._blogService = blogService;
            this._dateTimeHelper = dateTimeHelper;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare blog content model
        /// </summary>
        /// <param name="blogContentModel">Blog content model</param>
        /// <returns>Blog content model</returns>
        public virtual BlogContentModel PrepareBlogContentModel(BlogContentModel blogContentModel, int? filterByBlogPostId)
        {
            if (blogContentModel == null)
                throw new ArgumentNullException(nameof(blogContentModel));

            //prepare nested search models
            PrepareBlogPostSearchModel(blogContentModel.BlogPosts);
            var blogPost = _blogService.GetBlogPostById(filterByBlogPostId ?? 0);
            PrepareBlogCommentSearchModel(blogContentModel.BlogComments, blogPost);

            return blogContentModel;
        }

        /// <summary>
        /// Prepare blog post search model
        /// </summary>
        /// <param name="searchModel">Blog post search model</param>
        /// <returns>Blog post search model</returns>
        public virtual BlogPostSearchModel PrepareBlogPostSearchModel(BlogPostSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged blog post list model
        /// </summary>
        /// <param name="searchModel">Blog post search model</param>
        /// <returns>Blog post list model</returns>
        public virtual BlogPostListModel PrepareBlogPostListModel(BlogPostSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            
            //get blog posts
            var blogPosts = _blogService.GetAllBlogPosts(searchModel.SearchStoreId, showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new BlogPostListModel
            {
                Data = blogPosts.Select(blogPost =>
                {
                    //fill in model values from the entity
                    var blogPostModel = blogPost.ToModel<BlogPostModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    blogPostModel.Body = string.Empty;

                    //convert dates to the user time
                    if (blogPost.StartDateUtc.HasValue)
                        blogPostModel.StartDate = _dateTimeHelper.ConvertToUserTime(blogPost.StartDateUtc.Value, DateTimeKind.Utc);
                    if (blogPost.EndDateUtc.HasValue)
                        blogPostModel.EndDate = _dateTimeHelper.ConvertToUserTime(blogPost.EndDateUtc.Value, DateTimeKind.Utc);
                    blogPostModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogPost.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    blogPostModel.LanguageName = _languageService.GetLanguageById(blogPost.LanguageId)?.Name;
                    blogPostModel.ApprovedComments = _blogService.GetBlogCommentsCount(blogPost, isApproved: true);
                    blogPostModel.NotApprovedComments = _blogService.GetBlogCommentsCount(blogPost, isApproved: false);

                    return blogPostModel;
                }),
                Total = blogPosts.TotalCount
            };

            return model;
        }

        /// <summary>
        /// Prepare blog post model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Blog post model</returns>
        public virtual BlogPostModel PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (blogPost != null)
            {
                model = model ?? blogPost.ToModel<BlogPostModel>();
                model.StartDate = blogPost.StartDateUtc;
                model.EndDate = blogPost.EndDateUtc;
            }

            //set default values for the new model
            if (blogPost == null)
                model.AllowComments = true;

            //prepare available languages
            _baseAdminModelFactory.PrepareLanguages(model.AvailableLanguages, false);

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, blogPost, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare blog comment search model
        /// </summary>
        /// <param name="searchModel">Blog comment search model</param>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Blog comment search model</returns>
        public virtual BlogCommentSearchModel PrepareBlogCommentSearchModel(BlogCommentSearchModel searchModel, BlogPost blogPost)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.DisapprovedOnly"),
                Value = "2"
            });

            searchModel.BlogPostId = blogPost?.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged blog comment list model
        /// </summary>
        /// <param name="searchModel">Blog comment search model</param>
        /// <param name="blogPostId">Blog post ID</param>
        /// <returns>Blog comment list model</returns>
        public virtual BlogCommentListModel PrepareBlogCommentListModel(BlogCommentSearchModel searchModel, int? blogPostId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            
            //get parameters to filter comments
            var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);
            var createdOnToValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;

            //get comments
            var comments = _blogService.GetAllComments(blogPostId: blogPostId,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdOnToValue,
                commentText: searchModel.SearchText);

            //prepare store names (to avoid loading for each comment)
            var storeNames = _storeService.GetAllStores().ToDictionary(store => store.Id, store => store.Name);

            //prepare list model
            var model = new BlogCommentListModel
            {
                Data = comments.PaginationByRequestModel(searchModel).Select(blogComment =>
                {
                    //fill in model values from the entity
                    var commentModel = blogComment.ToModel<BlogCommentModel>();

                    //fill in additional values (not existing in the entity)
                    commentModel.CustomerInfo = blogComment.Customer.IsRegistered()
                        ? blogComment.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.Comment = HtmlHelper.FormatText(blogComment.CommentText, false, true, false, false, false, false);
                    commentModel.StoreName = storeNames.ContainsKey(blogComment.StoreId) ? storeNames[blogComment.StoreId] : "Deleted";

                    return commentModel;
                }),
                Total = comments.Count
            };

            return model;
        }

        #endregion
    }
}