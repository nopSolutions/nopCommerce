using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
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
        private readonly INotificationService _notificationService;
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
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService)
        {
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _newsModelFactory = newsModelFactory;
            _newsService = newsService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappings(NewsItem newsItem, NewsItemModel model)
        {
            newsItem.LimitedToStores = model.SelectedStoreIds.Any();
            await _newsService.UpdateNews(newsItem);

            var existingStoreMappings = await _storeMappingService.GetStoreMappings(newsItem);
            var allStores = await _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await _storeMappingService.InsertStoreMapping(newsItem, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Methods        

        #region News items

        public virtual IActionResult Index()
        {
            return RedirectToAction("NewsItems");
        }

        public virtual async Task<IActionResult> NewsItems(int? filterByNewsItemId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsContentModel(new NewsContentModel(), filterByNewsItemId);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(NewsItemSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsItemListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> NewsItemCreate()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsItemModel(new NewsItemModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> NewsItemCreate(NewsItemModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity<NewsItem>();
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                await _newsService.InsertNews(newsItem);

                //activity log
                await _customerActivityService.InsertActivity("AddNewNews",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = await _urlRecordService.ValidateSeName(newsItem, model.SeName, model.Title, true);
                await _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //Stores
                await SaveStoreMappings(newsItem, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Added"));

                if (!continueEditing)
                    return RedirectToAction("NewsItems");

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = await _newsModelFactory.PrepareNewsItemModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> NewsItemEdit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsById(id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            //prepare model
            var model = await _newsModelFactory.PrepareNewsItemModel(null, newsItem);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> NewsItemEdit(NewsItemModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsById(model.Id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            if (ModelState.IsValid)
            {
                newsItem = model.ToEntity(newsItem);
                await _newsService.UpdateNews(newsItem);

                //activity log
                await _customerActivityService.InsertActivity("EditNews",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = await _urlRecordService.ValidateSeName(newsItem, model.SeName, model.Title, true);
                await _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //stores
                await SaveStoreMappings(newsItem, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Updated"));

                if (!continueEditing)
                    return RedirectToAction("NewsItems");

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = await _newsModelFactory.PrepareNewsItemModel(model, newsItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsById(id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            await _newsService.DeleteNews(newsItem);

            //activity log
            await _customerActivityService.InsertActivity("DeleteNews",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Deleted"));

            return RedirectToAction("NewsItems");
        }

        #endregion

        #region Comments

        public virtual async Task<IActionResult> NewsComments(int? filterByNewsItemId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await _newsService.GetNewsById(filterByNewsItemId ?? 0);
            if (newsItem == null && filterByNewsItemId.HasValue)
                return RedirectToAction("NewsComments");

            //prepare model
            var model = await _newsModelFactory.PrepareNewsCommentSearchModel(new NewsCommentSearchModel(), newsItem);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Comments(NewsCommentSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _newsModelFactory.PrepareNewsCommentListModel(searchModel, searchModel.NewsItemId);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentUpdate(NewsCommentModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = await _newsService.GetNewsCommentById(model.Id)
                ?? throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            //fill entity from model
            comment = model.ToEntity(comment);

            await _newsService.UpdateNewsComment(comment);

            //activity log
            await _customerActivityService.InsertActivity("EditNewsComment",
                string.Format(await _localizationService.GetResource("ActivityLog.EditNewsComment"), comment.Id), comment);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                await _eventPublisher.Publish(new NewsCommentApprovedEvent(comment));

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = await _newsService.GetNewsCommentById(id)
                ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

            await _newsService.DeleteNewsComment(comment);

            //activity log
            await _customerActivityService.InsertActivity("DeleteNewsComment",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteNewsComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            var comments = await _newsService.GetNewsCommentsByIds(selectedIds.ToArray());

            await _newsService.DeleteNewsComments(comments);

            //activity log
            foreach (var newsComment in comments)
            {
                await _customerActivityService.InsertActivity("DeleteNewsComment",
                    string.Format(await _localizationService.GetResource("ActivityLog.DeleteNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter not approved comments
            var newsComments = (await _newsService.GetNewsCommentsByIds(selectedIds.ToArray())).Where(comment => !comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = true;

                await _newsService.UpdateNewsComment(newsComment);

                //raise event 
                await _eventPublisher.Publish(new NewsCommentApprovedEvent(newsComment));

                //activity log
                await _customerActivityService.InsertActivity("EditNewsComment",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            //filter approved comments
            var newsComments = (await _newsService.GetNewsCommentsByIds(selectedIds.ToArray())).Where(comment => comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = false;

                await _newsService.UpdateNewsComment(newsComment);

                //activity log
                await _customerActivityService.InsertActivity("EditNewsComment",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}