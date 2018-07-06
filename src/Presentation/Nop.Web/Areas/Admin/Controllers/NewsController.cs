using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class NewsController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INewsModelFactory _newsModelFactory;
        private readonly INewsService _newsService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public NewsController(ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INewsModelFactory newsModelFactory,
            INewsService newsService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._newsModelFactory = newsModelFactory;
            this._newsService = newsService;
            this._permissionService = permissionService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual void SaveStoreMappings(NewsItem newsItem, NewsItemModel model)
        {
            newsItem.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(newsItem);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(newsItem, store.Id);
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

        #region News items

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _newsModelFactory.PrepareNewsContentModel(new NewsContentModel(), filterByNewsItemId);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(NewsItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _newsModelFactory.PrepareNewsItemListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult NewsItemCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _newsModelFactory.PrepareNewsItemModel(new NewsItemModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult NewsItemCreate(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity<NewsItem>();
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                _newsService.InsertNews(newsItem);

                //activity log
                _customerActivityService.InsertActivity("AddNewNews",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(newsItem, model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //Stores
                SaveStoreMappings(newsItem, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = _newsModelFactory.PrepareNewsItemModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult NewsItemEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                return RedirectToAction("List");

            //prepare model
            var model = _newsModelFactory.PrepareNewsItemModel(null, newsItem);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult NewsItemEdit(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = _newsService.GetNewsById(model.Id);
            if (newsItem == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                newsItem = model.ToEntity(newsItem);
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;
                _newsService.UpdateNews(newsItem);

                //activity log
                _customerActivityService.InsertActivity("EditNews",
                    string.Format(_localizationService.GetResource("ActivityLog.EditNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(newsItem, model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //stores
                SaveStoreMappings(newsItem, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = _newsModelFactory.PrepareNewsItemModel(model, newsItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                return RedirectToAction("List");

            _newsService.DeleteNews(newsItem);

            //activity log
            _customerActivityService.InsertActivity("DeleteNews",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Comments

        public virtual IActionResult Comments(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = _newsService.GetNewsById(filterByNewsItemId ?? 0);
            if (newsItem == null && filterByNewsItemId.HasValue)
                return RedirectToAction("List");

            //prepare model
            var model = _newsModelFactory.PrepareNewsCommentSearchModel(new NewsCommentSearchModel(), newsItem);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Comments(NewsCommentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _newsModelFactory.PrepareNewsCommentListModel(searchModel, searchModel.NewsItemId);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CommentUpdate(NewsCommentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = _newsService.GetNewsCommentById(model.Id)
                ?? throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            comment.IsApproved = model.IsApproved;
            _newsService.UpdateNews(comment.NewsItem);

            //activity log
            _customerActivityService.InsertActivity("EditNewsComment",
                string.Format(_localizationService.GetResource("ActivityLog.EditNewsComment"), comment.Id), comment);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                _eventPublisher.Publish(new NewsCommentApprovedEvent(comment));

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult CommentDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = _newsService.GetNewsCommentById(id)
                ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

            _newsService.DeleteNewsComment(comment);

            //activity log
            _customerActivityService.InsertActivity("DeleteNewsComment",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteNewsComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            var comments = _newsService.GetNewsCommentsByIds(selectedIds.ToArray());

            _newsService.DeleteNewsComments(comments);

            //activity log
            foreach (var newsComment in comments)
            {
                _customerActivityService.InsertActivity("DeleteNewsComment",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter not approved comments
            var newsComments = _newsService.GetNewsCommentsByIds(selectedIds.ToArray()).Where(comment => !comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = true;
                _newsService.UpdateNews(newsComment.NewsItem);

                //raise event 
                _eventPublisher.Publish(new NewsCommentApprovedEvent(newsComment));

                //activity log
                _customerActivityService.InsertActivity("EditNewsComment",
                    string.Format(_localizationService.GetResource("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter approved comments
            var newsComments = _newsService.GetNewsCommentsByIds(selectedIds.ToArray()).Where(comment => comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = false;
                _newsService.UpdateNews(newsComment.NewsItem);

                //activity log
                _customerActivityService.InsertActivity("EditNewsComment",
                    string.Format(_localizationService.GetResource("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}