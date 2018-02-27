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
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the blog model factory implementation
    /// </summary>
    public partial class BlogModelFactory : IBlogModelFactory
    {
        #region Fields

        private readonly IBlogService _blogService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public BlogModelFactory(IBlogService blogService,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IStoreService storeService,
            IStoreMappingService storeMappingService)
        {
            this._blogService = blogService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare available languages for the passed model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post</param>
        protected virtual void PrepareModelLanguages(BlogPostModel model, BlogPost blogPost)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available languages
            var availableLanguages = _languageService.GetAllLanguages(showHidden: true);
            model.AvailableLanguages = availableLanguages.Select(language => new SelectListItem
            {
                Text = language.Name,
                Value = language.Id.ToString()
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available stores for the passed model
        /// </summary>
        /// <param name="model">Blog post model</param>
        /// <param name="blogPost">Blog post</param>
        /// <param name="ignoreStoreMappings">Whether to ignore existing store mappings</param>
        protected virtual void PrepareModelStores(BlogPostModel model, BlogPost blogPost, bool ignoreStoreMappings)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //try to get store identifiers with granted access
            if (!ignoreStoreMappings && blogPost != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(blogPost).ToList();

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            model.AvailableStores = availableStores.Select(store => new SelectListItem
            {
                Text = store.Name,
                Value = store.Id.ToString(),
                Selected = model.SelectedStoreIds.Contains(store.Id)
            }).ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare blog post list model
        /// </summary>
        /// <param name="model">Blog post list model</param>
        /// <returns>Blog post list model</returns>
        public virtual BlogPostListModel PrepareBlogPostListModel(BlogPostListModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            model.AvailableStores = availableStores
                .Select(store => new SelectListItem { Text = store.Name, Value = store.Id.ToString() }).ToList();

            //insert special store item for the "all" value
            model.AvailableStores.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return model;
        }

        /// <summary>
        /// Prepare paged blog post list model for the grid
        /// </summary>
        /// <param name="listModel">Blog post list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        public virtual DataSourceResult PrepareBlogPostListGridModel(BlogPostListModel listModel, DataSourceRequest command)
        {
            if (listModel == null)
                throw new ArgumentNullException(nameof(listModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            //get blog posts
            var blogPosts = _blogService.GetAllBlogPosts(listModel.SearchStoreId, showHidden: true,
                pageIndex: command.Page - 1, pageSize: command.PageSize);

            //prepare grid model
            var model = new DataSourceResult
            {
                Data = blogPosts.Select(blogPost =>
                {
                    //fill in model values from the entity
                    var blogPostModel = blogPost.ToModel();

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
                model = model ?? blogPost.ToModel();
                model.StartDate = blogPost.StartDateUtc;
                model.EndDate = blogPost.EndDateUtc;
            }

            //set default values for the new model
            if (blogPost == null)
                model.AllowComments = true;

            //prepare model languages
            PrepareModelLanguages(model, blogPost);

            //prepare model stores
            PrepareModelStores(model, blogPost, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare blog comment list model
        /// </summary>
        /// <param name="model">Blog comment list model</param>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Blog comment list model</returns>
        public virtual BlogCommentListModel PrepareBlogCommentListModel(BlogCommentListModel model, BlogPost blogPost)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            model.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.All"),
                Value = "0"
            });
            model.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            model.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.DisapprovedOnly"),
                Value = "2"
            });

            return model;
        }

        /// <summary>
        /// Prepare paged blog comment list model for the grid
        /// </summary>
        /// <param name="listModel">Blog comment list model</param>
        /// <param name="command">Pagination parameters</param>
        /// <param name="blogPost">Blog post; pass null to load comments of all posts</param>
        /// <returns>Grid model</returns>
        public virtual DataSourceResult PrepareBlogCommentListGridModel(BlogCommentListModel listModel,
            DataSourceRequest command, BlogPost blogPost)
        {
            if (listModel == null)
                throw new ArgumentNullException(nameof(listModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            //get parameters to filter comments
            var createdOnFromValue = listModel.CreatedOnFrom == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(listModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);
            var createdOnToValue = listModel.CreatedOnTo == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(listModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var isApprovedOnly = listModel.SearchApprovedId == 0 ? null : listModel.SearchApprovedId == 1 ? true : (bool?)false;

            //get comments
            var comments = _blogService.GetAllComments(blogPostId: blogPost?.Id,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdOnToValue,
                commentText: listModel.SearchText);

            //prepare store names (to avoid loading for each comment)
            var storeNames = _storeService.GetAllStores().ToDictionary(store => store.Id, store => store.Name);

            //prepare grid model
            var model = new DataSourceResult
            {
                Data = comments.PagedForCommand(command).Select(blogComment =>
                {
                    //fill in model values from the entity
                    var commentModel = new BlogCommentModel
                    {
                        Id = blogComment.Id,
                        BlogPostId = blogComment.BlogPostId,
                        BlogPostTitle = blogComment.BlogPost.Title,
                        CustomerId = blogComment.CustomerId,
                        IsApproved = blogComment.IsApproved,
                        StoreId = blogComment.StoreId
                    };

                    //fill in additional values (not existing in the entity)
                    commentModel.CustomerInfo = blogComment.Customer.IsRegistered()
                        ? blogComment.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.Comment = HtmlHelper.FormatText(blogComment.CommentText, false, true, false, false, false, false);
                    commentModel.StoreName = storeNames.ContainsKey(blogComment.StoreId) ? storeNames[blogComment.StoreId] : "Deleted";

                    return commentModel;
                }),
                Total = comments.Count,
            };

            return model;
        }

        #endregion
    }
}