using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Services.Blogs;
using Nop.Services.Events;
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

        private readonly IBlogModelFactory _blogModelFactory;
        private readonly IBlogService _blogService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

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
            _blogModelFactory = blogModelFactory;
            _blogService = blogService;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual void SaveStoreMappings(BlogPost blogPost, BlogPostModel model)
        {
            blogPost.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(blogPost);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(blogPost, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
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

        public virtual IActionResult BlogPosts(int? filterByBlogPostId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //prepare model
            var model = _blogModelFactory.PrepareBlogContentModel(new BlogContentModel(), filterByBlogPostId);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(BlogPostSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _blogModelFactory.PrepareBlogPostListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult BlogPostCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //prepare model
            var model = _blogModelFactory.PrepareBlogPostModel(new BlogPostModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult BlogPostCreate(BlogPostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var blogPost = model.ToEntity<BlogPost>();
                blogPost.CreatedOnUtc = DateTime.UtcNow;
                _blogService.InsertBlogPost(blogPost);

                //activity log
                _customerActivityService.InsertActivity("AddNewBlogPost",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewBlogPost"), blogPost.Id), blogPost);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(blogPost, model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(blogPost, seName, blogPost.LanguageId);

                //Stores
                SaveStoreMappings(blogPost, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Added"));

                if (!continueEditing)
                    return RedirectToAction("BlogPosts");
                
                return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
            }

            //prepare model
            model = _blogModelFactory.PrepareBlogPostModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult BlogPostEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = _blogService.GetBlogPostById(id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            //prepare model
            var model = _blogModelFactory.PrepareBlogPostModel(null, blogPost);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult BlogPostEdit(BlogPostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = _blogService.GetBlogPostById(model.Id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            if (ModelState.IsValid)
            {
                blogPost = model.ToEntity(blogPost);
                _blogService.UpdateBlogPost(blogPost);

                //activity log
                _customerActivityService.InsertActivity("EditBlogPost",
                    string.Format(_localizationService.GetResource("ActivityLog.EditBlogPost"), blogPost.Id), blogPost);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(blogPost, model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(blogPost, seName, blogPost.LanguageId);

                //Stores
                SaveStoreMappings(blogPost, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Updated"));

                if (!continueEditing)
                    return RedirectToAction("BlogPosts");
                
                return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
            }

            //prepare model
            model = _blogModelFactory.PrepareBlogPostModel(model, blogPost, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = _blogService.GetBlogPostById(id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            _blogService.DeleteBlogPost(blogPost);

            //activity log
            _customerActivityService.InsertActivity("DeleteBlogPost",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteBlogPost"), blogPost.Id), blogPost);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Deleted"));

            return RedirectToAction("BlogPosts");
        }

        #endregion

        #region Comments

        public virtual IActionResult BlogComments(int? filterByBlogPostId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = _blogService.GetBlogPostById(filterByBlogPostId ?? 0);
            if (blogPost == null && filterByBlogPostId.HasValue)
                return RedirectToAction("BlogComments");

            //prepare model
            var model = _blogModelFactory.PrepareBlogCommentSearchModel(new BlogCommentSearchModel(), blogPost);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Comments(BlogCommentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _blogModelFactory.PrepareBlogCommentListModel(searchModel, searchModel.BlogPostId);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CommentUpdate(BlogCommentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog comment with the specified id
            var comment = _blogService.GetBlogCommentById(model.Id)
                ?? throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            //fill entity from model
            comment = model.ToEntity(comment);
            _blogService.UpdateBlogPost(comment.BlogPost);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                _eventPublisher.Publish(new BlogCommentApprovedEvent(comment));

            //activity log
            _customerActivityService.InsertActivity("EditBlogComment",
               string.Format(_localizationService.GetResource("ActivityLog.EditBlogComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        public virtual IActionResult CommentDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog comment with the specified id
            var comment = _blogService.GetBlogCommentById(id)
                ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

            _blogService.DeleteBlogComment(comment);

            //activity log
            _customerActivityService.InsertActivity("DeleteBlogPostComment",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteBlogPostComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            var comments = _blogService.GetBlogCommentsByIds(selectedIds.ToArray());

            _blogService.DeleteBlogComments(comments);
            //activity log
            foreach (var blogComment in comments)
            {
                _customerActivityService.InsertActivity("DeleteBlogPostComment",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteBlogPostComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter not approved comments
            var blogComments = _blogService.GetBlogCommentsByIds(selectedIds.ToArray()).Where(comment => !comment.IsApproved);

            foreach (var blogComment in blogComments)
            {
                blogComment.IsApproved = true;
                _blogService.UpdateBlogPost(blogComment.BlogPost);

                //raise event 
                _eventPublisher.Publish(new BlogCommentApprovedEvent(blogComment));

                //activity log
                _customerActivityService.InsertActivity("EditBlogComment",
                    string.Format(_localizationService.GetResource("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter approved comments
            var blogComments = _blogService.GetBlogCommentsByIds(selectedIds.ToArray()).Where(comment => comment.IsApproved);

            foreach (var blogComment in blogComments)
            {
                blogComment.IsApproved = false;
                _blogService.UpdateBlogPost(blogComment.BlogPost);

                //activity log
                _customerActivityService.InsertActivity("EditBlogComment",
                    string.Format(_localizationService.GetResource("ActivityLog.EditBlogComment"), blogComment.Id), blogComment);
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}