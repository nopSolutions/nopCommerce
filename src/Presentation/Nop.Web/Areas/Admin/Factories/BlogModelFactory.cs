using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Blogs;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the blog model factory implementation
    /// </summary>
    public partial class BlogModelFactory : IBlogModelFactory
    {
        #region Fields

        protected CatalogSettings CatalogSettings { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected IBlogService BlogService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INopHtmlHelper NopHtmlHelper { get; }
        protected IStoreMappingSupportedModelFactory StoreMappingSupportedModelFactory { get; }
        protected IStoreService StoreService { get; }
        protected IUrlRecordService UrlRecordService { get; }

        #endregion

        #region Ctor

        public BlogModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IBlogService blogService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INopHtmlHelper nopHtmlHelper,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            CatalogSettings = catalogSettings;
            BaseAdminModelFactory = baseAdminModelFactory;
            BlogService = blogService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            LanguageService = languageService;
            LocalizationService = localizationService;
            NopHtmlHelper = nopHtmlHelper;
            StoreMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            StoreService = storeService;
            UrlRecordService = urlRecordService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare blog content model
        /// </summary>
        /// <param name="blogContentModel">Blog content model</param>
        /// <param name="filterByBlogPostId">Blog post ID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog content model
        /// </returns>
        public virtual async Task<BlogContentModel> PrepareBlogContentModelAsync(BlogContentModel blogContentModel, int? filterByBlogPostId)
        {
            if (blogContentModel == null)
                throw new ArgumentNullException(nameof(blogContentModel));

            //prepare nested search models
            await PrepareBlogPostSearchModelAsync(blogContentModel.BlogPosts);
            var blogPost = await BlogService.GetBlogPostByIdAsync(filterByBlogPostId ?? 0);
            await PrepareBlogCommentSearchModelAsync(blogContentModel.BlogComments, blogPost);

            return blogContentModel;
        }
        
        /// <summary>
        /// Prepare paged blog post list model
        /// </summary>
        /// <param name="searchModel">Blog post search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog post list model
        /// </returns>
        public virtual async Task<BlogPostListModel> PrepareBlogPostListModelAsync(BlogPostSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get blog posts
            var blogPosts = await BlogService.GetAllBlogPostsAsync(storeId: searchModel.SearchStoreId, showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, title: searchModel.SearchTitle);

            //prepare list model
            var model = await new BlogPostListModel().PrepareToGridAsync(searchModel, blogPosts, () =>
            {
                return blogPosts.SelectAwait(async blogPost =>
                {
                    //fill in model values from the entity
                    var blogPostModel = blogPost.ToModel<BlogPostModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    blogPostModel.Body = string.Empty;

                    //convert dates to the user time
                    if (blogPost.StartDateUtc.HasValue)
                        blogPostModel.StartDateUtc = await DateTimeHelper.ConvertToUserTimeAsync(blogPost.StartDateUtc.Value, DateTimeKind.Utc);
                    if (blogPost.EndDateUtc.HasValue)
                        blogPostModel.EndDateUtc = await DateTimeHelper.ConvertToUserTimeAsync(blogPost.EndDateUtc.Value, DateTimeKind.Utc);
                    blogPostModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(blogPost.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    blogPostModel.LanguageName = (await LanguageService.GetLanguageByIdAsync(blogPost.LanguageId))?.Name;
                    blogPostModel.ApprovedComments = await BlogService.GetBlogCommentsCountAsync(blogPost, isApproved: true);
                    blogPostModel.NotApprovedComments = await BlogService.GetBlogCommentsCountAsync(blogPost, isApproved: false);
                    blogPostModel.SeName = await UrlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, true, false);

                    return blogPostModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare blog post model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog post model
        /// </returns>
        public virtual async Task<BlogPostModel> PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (blogPost != null)
            {
                if (model == null)
                {
                    model = blogPost.ToModel<BlogPostModel>();
                    model.SeName = await UrlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, true, false);
                }
                model.StartDateUtc = blogPost.StartDateUtc;
                model.EndDateUtc = blogPost.EndDateUtc;
            }

            //set default values for the new model
            if (blogPost == null)
            {
                model.AllowComments = true;
                model.IncludeInSitemap = true;
            }

            var blogTags = await BlogService.GetAllBlogPostTagsAsync(0, 0, true);
            var blogTagsSb = new StringBuilder();
            blogTagsSb.Append("var initialBlogTags = [");
            for (var i = 0; i < blogTags.Count; i++)
            {
                var tag = blogTags[i];
                blogTagsSb.Append('\'');
                blogTagsSb.Append(JavaScriptEncoder.Default.Encode(tag.Name));
                blogTagsSb.Append('\'');
                if (i != blogTags.Count - 1)
                    blogTagsSb.Append(',');
            }
            blogTagsSb.Append(']');

            model.InitialBlogTags = blogTagsSb.ToString();

            //prepare available languages
            await BaseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

            //prepare available stores
            await StoreMappingSupportedModelFactory.PrepareModelStoresAsync(model, blogPost, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare blog comment search model
        /// </summary>
        /// <param name="searchModel">Blog comment search model</param>
        /// <param name="blogPost">Blog post</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog comment search model
        /// </returns>
        public virtual async Task<BlogCommentSearchModel> PrepareBlogCommentSearchModelAsync(BlogCommentSearchModel searchModel, BlogPost blogPost)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.ContentManagement.Blog.Comments.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.ContentManagement.Blog.Comments.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.ContentManagement.Blog.Comments.List.SearchApproved.DisapprovedOnly"),
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog comment list model
        /// </returns>
        public virtual async Task<BlogCommentListModel> PrepareBlogCommentListModelAsync(BlogCommentSearchModel searchModel, int? blogPostId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var createdOnFromValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());
            var createdOnToValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;

            //get comments
            var comments = (await BlogService.GetAllCommentsAsync(blogPostId: blogPostId,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdOnToValue,
                commentText: searchModel.SearchText)).ToPagedList(searchModel);

            //prepare store names (to avoid loading for each comment)
            var storeNames = (await StoreService.GetAllStoresAsync())
                .ToDictionary(store => store.Id, store => store.Name);

            //prepare list model
            var model = await new BlogCommentListModel().PrepareToGridAsync(searchModel, comments, () =>
            {
                return comments.SelectAwait(async blogComment =>
                {
                    //fill in model values from the entity
                    var commentModel = blogComment.ToModel<BlogCommentModel>();

                    //set title from linked blog post
                    commentModel.BlogPostTitle = (await BlogService.GetBlogPostByIdAsync(blogComment.BlogPostId))?.Title;

                    if ((await CustomerService.GetCustomerByIdAsync(blogComment.CustomerId)) is Customer customer)
                    {
                        commentModel.CustomerInfo = (await CustomerService.IsRegisteredAsync(customer))
                            ? customer.Email
                            : await LocalizationService.GetResourceAsync("Admin.Customers.Guest");
                    }
                    //fill in additional values (not existing in the entity)
                    commentModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(blogComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.Comment = NopHtmlHelper.FormatText(blogComment.CommentText, false, true, false, false, false, false);
                    commentModel.StoreName = storeNames.ContainsKey(blogComment.StoreId) ? storeNames[blogComment.StoreId] : "Deleted";

                    return commentModel;
                });
            });

            return model;
        }
        
        /// <summary>
        /// Prepare blog post search model
        /// </summary>
        /// <param name="searchModel">Blog post search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the blog post search model
        /// </returns>
        public virtual async Task<BlogPostSearchModel> PrepareBlogPostSearchModelAsync(BlogPostSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = CatalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion
    }
}