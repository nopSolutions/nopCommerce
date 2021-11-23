using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Core.Events;
using Nop.Services.Blogs;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class BlogController : BaseAdminController
    {
        #region Fields

        protected IBlogModelFactory BlogModelFactory { get; }
        protected IBlogService BlogService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }
        protected IUrlRecordService UrlRecordService { get; }

        #endregion

        #region Ctor

        public BlogController(IBlogModelFactory blogModelFactory,
            IBlogService blogService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            BlogModelFactory = blogModelFactory;
            BlogService = blogService;
            CustomerActivityService = customerActivityService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            UrlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappingsAsync(BlogPost blogPost, BlogPostModel model)
        {
            blogPost.LimitedToStores = model.SelectedStoreIds.Any();
            await BlogService.UpdateBlogPostAsync(blogPost);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(blogPost);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await StoreMappingService.InsertStoreMappingAsync(blogPost, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await StoreMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Methods        

        #region Blog posts

        public virtual IActionResult Index()
        {
            return RedirectToAction("BlogPosts");
        }

        public virtual async Task<IActionResult> BlogPosts(int? filterByBlogPostId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //prepare model
            var model = await BlogModelFactory.PrepareBlogContentModelAsync(new BlogContentModel(), filterByBlogPostId);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(BlogPostSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await BlogModelFactory.PrepareBlogPostListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> BlogPostCreate()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //prepare model
            var model = await BlogModelFactory.PrepareBlogPostModelAsync(new BlogPostModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> BlogPostCreate(BlogPostModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var blogPost = model.ToEntity<BlogPost>();
                blogPost.CreatedOnUtc = DateTime.UtcNow;
                await BlogService.InsertBlogPostAsync(blogPost);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewBlogPost",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewBlogPost"), blogPost.Id), blogPost);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
                await UrlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

                //Stores
                await SaveStoreMappingsAsync(blogPost, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Added"));

                if (!continueEditing)
                    return RedirectToAction("BlogPosts");

                return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
            }

            //prepare model
            model = await BlogModelFactory.PrepareBlogPostModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> BlogPostEdit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await BlogService.GetBlogPostByIdAsync(id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            //prepare model
            var model = await BlogModelFactory.PrepareBlogPostModelAsync(null, blogPost);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> BlogPostEdit(BlogPostModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await BlogService.GetBlogPostByIdAsync(model.Id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            if (ModelState.IsValid)
            {
                blogPost = model.ToEntity(blogPost);
                await BlogService.UpdateBlogPostAsync(blogPost);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditBlogPost",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditBlogPost"), blogPost.Id), blogPost);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
                await UrlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

                //Stores
                await SaveStoreMappingsAsync(blogPost, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Updated"));

                if (!continueEditing)
                    return RedirectToAction("BlogPosts");

                return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
            }

            //prepare model
            model = await BlogModelFactory.PrepareBlogPostModelAsync(model, blogPost, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await BlogService.GetBlogPostByIdAsync(id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            await BlogService.DeleteBlogPostAsync(blogPost);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteBlogPost",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteBlogPost"), blogPost.Id), blogPost);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Deleted"));

            return RedirectToAction("BlogPosts");
        }

        #endregion

        #region Comments

        public virtual async Task<IActionResult> BlogComments(int? filterByBlogPostId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await BlogService.GetBlogPostByIdAsync(filterByBlogPostId ?? 0);
            if (blogPost == null && filterByBlogPostId.HasValue)
                return RedirectToAction("BlogComments");

            //prepare model
            var model =await BlogModelFactory.PrepareBlogCommentSearchModelAsync(new BlogCommentSearchModel(), blogPost);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Comments(BlogCommentSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await BlogModelFactory.PrepareBlogCommentListModelAsync(searchModel, searchModel.BlogPostId);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentUpdate(BlogCommentModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog comment with the specified id
            var comment = await BlogService.GetBlogCommentByIdAsync(model.Id)
                ?? throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            //fill entity from model
            comment = model.ToEntity(comment);

            await BlogService.UpdateBlogCommentAsync(comment);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                await EventPublisher.PublishAsync(new BlogCommentApprovedEvent(comment));

            //activity log
            await CustomerActivityService.InsertActivityAsync("EditBlogComment",
               string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditBlogComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CommentDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog comment with the specified id
            var comment = await BlogService.GetBlogCommentByIdAsync(id)
                ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

            await BlogService.DeleteBlogCommentAsync(comment);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteBlogPostComment",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var comments = await BlogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray());

            await BlogService.DeleteBlogCommentsAsync(comments);
            //activity log
            foreach (var blogComment in comments)
            {
                await CustomerActivityService.InsertActivityAsync("DeleteBlogPostComment",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteBlogPostComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            //filter not approved comments
            var blogComments = (await BlogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved);

            foreach (var blogComment in blogComments)
            {
                blogComment.IsApproved = true;

                await BlogService.UpdateBlogCommentAsync(blogComment);

                //raise event 
                await EventPublisher.PublishAsync(new BlogCommentApprovedEvent(blogComment));

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditBlogComment",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            //filter approved comments
            var blogComments = (await BlogService.GetBlogCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved);

            foreach (var blogComment in blogComments)
            {
                blogComment.IsApproved = false;

                await BlogService.UpdateBlogCommentAsync(blogComment);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditBlogComment",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}