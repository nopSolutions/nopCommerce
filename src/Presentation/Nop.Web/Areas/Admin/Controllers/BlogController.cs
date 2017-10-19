using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Customers;
using Nop.Services.Blogs;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class BlogController : BaseAdminController
	{
        #region Fields

        private readonly IBlogService _blogService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICustomerActivityService _customerActivityService;
        
        #endregion
        
        #region Ctor

        public BlogController(IBlogService blogService,
            ILanguageService languageService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            ICustomerActivityService customerActivityService)
        {
            this._blogService = blogService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._customerActivityService = customerActivityService;
        }
        
        #endregion
        
        #region Utilities
        
        protected virtual void PrepareLanguagesModel(BlogPostModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var languages = _languageService.GetAllLanguages(true);
            foreach (var language in languages)
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Text = language.Name,
                    Value = language.Id.ToString()
                });
            }
        }
        
        protected virtual void PrepareStoresMappingModel(BlogPostModel model, BlogPost blogPost, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && blogPost != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(blogPost).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }
        
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
        
        #region Blog posts

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

		public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var model = new BlogPostListModel();

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            return View(model);
		}

		[HttpPost]
        public virtual IActionResult List(DataSourceRequest command, BlogPostListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedKendoGridJson();

            var blogPosts = _blogService.GetAllBlogPosts(model.SearchStoreId, 0, null, null, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = blogPosts.Select(x =>
                {
                    var m = x.ToModel();
                    //little performance optimization: ensure that "Body" is not returned
                    m.Body = "";
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = _languageService.GetLanguageById(x.LanguageId)?.Name;
                    m.ApprovedComments = _blogService.GetBlogCommentsCount(x, isApproved: true);
                    m.NotApprovedComments = _blogService.GetBlogCommentsCount(x, isApproved: false);

                    return m;
                }),
                Total = blogPosts.TotalCount
            };

            return Json(gridModel);
		}
        
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();
            
            var model = new BlogPostModel();
            //languages
            PrepareLanguagesModel(model);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.AllowComments = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(BlogPostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var blogPost = model.ToEntity();
                blogPost.StartDateUtc = model.StartDate;
                blogPost.EndDateUtc = model.EndDate;
                blogPost.CreatedOnUtc = DateTime.UtcNow;
                _blogService.InsertBlogPost(blogPost);

                //activity log
                _customerActivityService.InsertActivity("AddNewBlogPost", _localizationService.GetResource("ActivityLog.AddNewBlogPost"), blogPost.Id);

                //search engine name
                var seName = blogPost.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(blogPost, seName, blogPost.LanguageId);

                //Stores
                SaveStoreMappings(blogPost, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = blogPost.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareLanguagesModel(model);
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

		public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var blogPost = _blogService.GetBlogPostById(id);
            if (blogPost == null)
                //No blog post found with the specified id
                return RedirectToAction("List");

            var model = blogPost.ToModel();
            model.StartDate = blogPost.StartDateUtc;
            model.EndDate = blogPost.EndDateUtc;
            //languages
            PrepareLanguagesModel(model);
            //Store
            PrepareStoresMappingModel(model, blogPost, false);
            return View(model);
		}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
		public virtual IActionResult Edit(BlogPostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var blogPost = _blogService.GetBlogPostById(model.Id);
            if (blogPost == null)
                //No blog post found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                blogPost = model.ToEntity(blogPost);
                blogPost.StartDateUtc = model.StartDate;
                blogPost.EndDateUtc = model.EndDate;
                _blogService.UpdateBlogPost(blogPost);

                //activity log
                _customerActivityService.InsertActivity("EditBlogPost", _localizationService.GetResource("ActivityLog.EditBlogPost"), blogPost.Id);

                //search engine name
                var seName = blogPost.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(blogPost, seName, blogPost.LanguageId);

                //Stores
                SaveStoreMappings(blogPost, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = blogPost.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareStoresMappingModel(model, blogPost, true);
            PrepareLanguagesModel(model);
            return View(model);
		}

		[HttpPost]
		public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var blogPost = _blogService.GetBlogPostById(id);
            if (blogPost == null)
                //No blog post found with the specified id
                return RedirectToAction("List");

            _blogService.DeleteBlogPost(blogPost);

            //activity log
            _customerActivityService.InsertActivity("DeleteBlogPost", _localizationService.GetResource("ActivityLog.DeleteBlogPost"), blogPost.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Deleted"));
			return RedirectToAction("List");
		}
        
        #endregion
        
        #region Comments

        public virtual IActionResult Comments(int? filterByBlogPostId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            ViewBag.FilterByBlogPostId = filterByBlogPostId;
            var model = new BlogCommentListModel();

            //"approved" property
            //0 - all
            //1 - approved only
            //2 - disapproved only
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.All"), Value = "0" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.ApprovedOnly"), Value = "1" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.ContentManagement.Blog.Comments.List.SearchApproved.DisapprovedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Comments(int? filterByBlogPostId, DataSourceRequest command, BlogCommentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedKendoGridJson();

            var createdOnFromValue = model.CreatedOnFrom == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            var createdOnToValue = model.CreatedOnTo == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            bool? approved = null;
            if (model.SearchApprovedId > 0)
                approved = model.SearchApprovedId == 1;

            var comments = _blogService.GetAllComments(0, 0, filterByBlogPostId, approved, createdOnFromValue, createdOnToValue, model.SearchText);

            var storeNames = _storeService.GetAllStores().ToDictionary(store => store.Id, store => store.Name);

            var gridModel = new DataSourceResult
            {
                Data = comments.PagedForCommand(command).Select(blogComment =>
                {
                    var commentModel = new BlogCommentModel
                    {
                        Id = blogComment.Id,
                        BlogPostId = blogComment.BlogPostId,
                        BlogPostTitle = blogComment.BlogPost.Title,
                        CustomerId = blogComment.CustomerId
                    };
                    var customer = blogComment.Customer;
                    commentModel.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.Comment = Core.Html.HtmlHelper.FormatText(blogComment.CommentText, false, true, false, false, false, false);
                    commentModel.IsApproved = blogComment.IsApproved;
                    commentModel.StoreId = blogComment.StoreId;
                    commentModel.StoreName = storeNames.ContainsKey(blogComment.StoreId) ? storeNames[blogComment.StoreId] : "Deleted";

                    return commentModel;
                }),
                Total = comments.Count,
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult CommentUpdate(BlogCommentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var comment = _blogService.GetBlogCommentById(model.Id);
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            comment.IsApproved = model.IsApproved;
            _blogService.UpdateBlogPost(comment.BlogPost);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                _eventPublisher.Publish(new BlogCommentApprovedEvent(comment));

            //activity log
            _customerActivityService.InsertActivity("EditBlogComment", _localizationService.GetResource("ActivityLog.EditBlogComment"), model.Id);

            return new NullJsonResult();
        }

        public virtual IActionResult CommentDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            var comment = _blogService.GetBlogCommentById(id);
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var blogPost = comment.BlogPost;
            _blogService.DeleteBlogComment(comment);

            //activity log
            _customerActivityService.InsertActivity("DeleteBlogPostComment", _localizationService.GetResource("ActivityLog.DeleteBlogPostComment"), blogPost.Id);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var comments = _blogService.GetBlogCommentsByIds(selectedIds.ToArray());

                _blogService.DeleteBlogComments(comments);
                //activity log
                foreach (var blogComment in comments)
                {
                    _customerActivityService.InsertActivity("DeleteBlogPostComment", _localizationService.GetResource("ActivityLog.DeleteBlogPostComment"), blogComment.Id);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                //filter not approved comments
                var blogComments = _blogService.GetBlogCommentsByIds(selectedIds.ToArray()).Where(comment => !comment.IsApproved);

                foreach (var blogComment in blogComments)
                {
                    blogComment.IsApproved = true;
                    _blogService.UpdateBlogPost(blogComment.BlogPost);

                    //raise event 
                    _eventPublisher.Publish(new BlogCommentApprovedEvent(blogComment));

                    //activity log
                    _customerActivityService.InsertActivity("EditBlogComment", _localizationService.GetResource("ActivityLog.EditBlogComment"), blogComment.Id);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                //filter approved comments
                var blogComments = _blogService.GetBlogCommentsByIds(selectedIds.ToArray()).Where(comment => comment.IsApproved);

                foreach (var blogComment in blogComments)
                {
                    blogComment.IsApproved = false;
                    _blogService.UpdateBlogPost(blogComment.BlogPost);

                    //activity log
                    _customerActivityService.InsertActivity("EditBlogComment", _localizationService.GetResource("ActivityLog.EditBlogComment"), blogComment.Id);
                }
            }

            return Json(new { Result = true });
        }
        
        #endregion
    }
}