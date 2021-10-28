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

        protected ICustomerActivityService CustomerActivityService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INewsModelFactory NewsModelFactory { get; }
        protected INewsService NewsService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }
        protected IUrlRecordService UrlRecordService { get; }

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
            CustomerActivityService = customerActivityService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            NewsModelFactory = newsModelFactory;
            NewsService = newsService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
            UrlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappingsAsync(NewsItem newsItem, NewsItemModel model)
        {
            newsItem.LimitedToStores = model.SelectedStoreIds.Any();
            await NewsService.UpdateNewsAsync(newsItem);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(newsItem);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await StoreMappingService.InsertStoreMappingAsync(newsItem, store.Id);
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

        #region News items

        public virtual IActionResult Index()
        {
            return RedirectToAction("NewsItems");
        }

        public virtual async Task<IActionResult> NewsItems(int? filterByNewsItemId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = await NewsModelFactory.PrepareNewsContentModelAsync(new NewsContentModel(), filterByNewsItemId);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(NewsItemSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await NewsModelFactory.PrepareNewsItemListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> NewsItemCreate()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = await NewsModelFactory.PrepareNewsItemModelAsync(new NewsItemModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> NewsItemCreate(NewsItemModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity<NewsItem>();
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                await NewsService.InsertNewsAsync(newsItem);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewNews",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
                await UrlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

                //Stores
                await SaveStoreMappingsAsync(newsItem, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Added"));

                if (!continueEditing)
                    return RedirectToAction("NewsItems");

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = await NewsModelFactory.PrepareNewsItemModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> NewsItemEdit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await NewsService.GetNewsByIdAsync(id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            //prepare model
            var model = await NewsModelFactory.PrepareNewsItemModelAsync(null, newsItem);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> NewsItemEdit(NewsItemModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await NewsService.GetNewsByIdAsync(model.Id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            if (ModelState.IsValid)
            {
                newsItem = model.ToEntity(newsItem);
                await NewsService.UpdateNewsAsync(newsItem);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditNews",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditNews"), newsItem.Id), newsItem);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(newsItem, model.SeName, model.Title, true);
                await UrlRecordService.SaveSlugAsync(newsItem, seName, newsItem.LanguageId);

                //stores
                await SaveStoreMappingsAsync(newsItem, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Updated"));

                if (!continueEditing)
                    return RedirectToAction("NewsItems");

                return RedirectToAction("NewsItemEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = await NewsModelFactory.PrepareNewsItemModelAsync(model, newsItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await NewsService.GetNewsByIdAsync(id);
            if (newsItem == null)
                return RedirectToAction("NewsItems");

            await NewsService.DeleteNewsAsync(newsItem);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteNews",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteNews"), newsItem.Id), newsItem);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Deleted"));

            return RedirectToAction("NewsItems");
        }

        #endregion

        #region Comments

        public virtual async Task<IActionResult> NewsComments(int? filterByNewsItemId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = await NewsService.GetNewsByIdAsync(filterByNewsItemId ?? 0);
            if (newsItem == null && filterByNewsItemId.HasValue)
                return RedirectToAction("NewsComments");

            //prepare model
            var model = await NewsModelFactory.PrepareNewsCommentSearchModelAsync(new NewsCommentSearchModel(), newsItem);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Comments(NewsCommentSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await NewsModelFactory.PrepareNewsCommentListModelAsync(searchModel, searchModel.NewsItemId);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentUpdate(NewsCommentModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = await NewsService.GetNewsCommentByIdAsync(model.Id)
                ?? throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            //fill entity from model
            comment = model.ToEntity(comment);

            await NewsService.UpdateNewsCommentAsync(comment);

            //activity log
            await CustomerActivityService.InsertActivityAsync("EditNewsComment",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditNewsComment"), comment.Id), comment);

            //raise event (only if it wasn't approved before and is approved now)
            if (!previousIsApproved && comment.IsApproved)
                await EventPublisher.PublishAsync(new NewsCommentApprovedEvent(comment));

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CommentDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news comment with the specified id
            var comment = await NewsService.GetNewsCommentByIdAsync(id)
                ?? throw new ArgumentException("No comment found with the specified id", nameof(id));

            await NewsService.DeleteNewsCommentAsync(comment);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteNewsComment",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), comment.Id), comment);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            var comments = await NewsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray());

            await NewsService.DeleteNewsCommentsAsync(comments);

            //activity log
            foreach (var newsComment in comments)
            {
                await CustomerActivityService.InsertActivityAsync("DeleteNewsComment",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            //filter not approved comments
            var newsComments = (await NewsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => !comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = true;

                await NewsService.UpdateNewsCommentAsync(newsComment);

                //raise event 
                await EventPublisher.PublishAsync(new NewsCommentApprovedEvent(newsComment));

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditNewsComment",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count() == 0)
                return NoContent();

            //filter approved comments
            var newsComments = (await NewsService.GetNewsCommentsByIdsAsync(selectedIds.ToArray())).Where(comment => comment.IsApproved);

            foreach (var newsComment in newsComments)
            {
                newsComment.IsApproved = false;

                await NewsService.UpdateNewsCommentAsync(newsComment);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditNewsComment",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditNewsComment"), newsComment.Id), newsComment);
            }

            return Json(new { Result = true });
        }

        #endregion

        #endregion
    }
}