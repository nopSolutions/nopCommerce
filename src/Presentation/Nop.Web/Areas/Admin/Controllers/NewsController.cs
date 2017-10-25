using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.News;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class NewsController : BaseAdminController
	{
		#region Fields

        private readonly INewsService _newsService;
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

        public NewsController(INewsService newsService, 
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
            this._newsService = newsService;
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

        protected virtual void PrepareLanguagesModel(NewsItemModel model)
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

        protected virtual void PrepareStoresMappingModel(NewsItemModel model, NewsItem newsItem, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && newsItem != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(newsItem).ToList();

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

        #region News items

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var model = new NewsItemListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, NewsItemListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            var news = _newsService.GetAllNews(0, model.SearchStoreId, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = news.Select(x =>
                {
                    var m = x.ToModel();
                    //little performance optimization: ensure that "Full" is not returned
                    m.Full = "";
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = _languageService.GetLanguageById(x.LanguageId)?.Name;
                    m.ApprovedComments = _newsService.GetNewsCommentsCount(x, isApproved: true);
                    m.NotApprovedComments = _newsService.GetNewsCommentsCount(x, isApproved: false);

                    return m;
                }),
                Total = news.TotalCount
            };

            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var model = new NewsItemModel();
            //languages
            PrepareLanguagesModel(model);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.Published = true;
            model.AllowComments = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity();
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                _newsService.InsertNews(newsItem);

                //activity log
                _customerActivityService.InsertActivity("AddNewNews", _localizationService.GetResource("ActivityLog.AddNewNews"), newsItem.Id);

                //search engine name
                var seName = newsItem.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //Stores
                SaveStoreMappings(newsItem, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = newsItem.Id });
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            var model = newsItem.ToModel();
            model.StartDate = newsItem.StartDateUtc;
            model.EndDate = newsItem.EndDateUtc;
            //languages
            PrepareLanguagesModel(model);
            //Store
            PrepareStoresMappingModel(model, newsItem, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(NewsItemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(model.Id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                newsItem = model.ToEntity(newsItem);
                newsItem.StartDateUtc = model.StartDate;
                newsItem.EndDateUtc = model.EndDate;
                _newsService.UpdateNews(newsItem);

                //activity log
                _customerActivityService.InsertActivity("EditNews", _localizationService.GetResource("ActivityLog.EditNews"), newsItem.Id);

                //search engine name
                var seName = newsItem.ValidateSeName(model.SeName, model.Title, true);
                _urlRecordService.SaveSlug(newsItem, seName, newsItem.LanguageId);

                //Stores
                SaveStoreMappings(newsItem, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = newsItem.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareLanguagesModel(model);
            PrepareStoresMappingModel(model, newsItem, true);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            _newsService.DeleteNews(newsItem);

            //activity log
            _customerActivityService.InsertActivity("DeleteNews", _localizationService.GetResource("ActivityLog.DeleteNews"), newsItem.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Comments

        public virtual IActionResult Comments(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            ViewBag.FilterByNewsItemId = filterByNewsItemId;
            var model = new NewsCommentListModel();

            //"approved" property
            //0 - all
            //1 - approved only
            //2 - disapproved only
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.ContentManagement.News.Comments.List.SearchApproved.All"), Value = "0" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.ContentManagement.News.Comments.List.SearchApproved.ApprovedOnly"), Value = "1" });
            model.AvailableApprovedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.ContentManagement.News.Comments.List.SearchApproved.DisapprovedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Comments(int? filterByNewsItemId, DataSourceRequest command, NewsCommentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedKendoGridJson();

            var createdOnFromValue = model.CreatedOnFrom == null ? null
                           : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);

            var createdOnToValue = model.CreatedOnTo == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            bool? approved = null;
            if (model.SearchApprovedId > 0)
                approved = model.SearchApprovedId == 1;

            var comments = _newsService.GetAllComments(0, 0, filterByNewsItemId, approved, createdOnFromValue, createdOnToValue, model.SearchText);

            var storeNames = _storeService.GetAllStores().ToDictionary(store => store.Id, store => store.Name);

            var gridModel = new DataSourceResult
            {
                Data = comments.PagedForCommand(command).Select(newsComment =>
                {
                    var commentModel = new NewsCommentModel
                    {
                        Id = newsComment.Id,
                        NewsItemId = newsComment.NewsItemId,
                        NewsItemTitle = newsComment.NewsItem.Title,
                        CustomerId = newsComment.CustomerId
                    };
                    var customer = newsComment.Customer;
                    commentModel.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.CommentTitle = newsComment.CommentTitle;
                    commentModel.CommentText = Core.Html.HtmlHelper.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    commentModel.IsApproved = newsComment.IsApproved;
                    commentModel.StoreId = newsComment.StoreId;
                    commentModel.StoreName = storeNames.ContainsKey(newsComment.StoreId) ? storeNames[newsComment.StoreId] : "Deleted";

                    return commentModel;
                }),
                Total = comments.Count,
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult CommentUpdate(NewsCommentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var comment = _newsService.GetNewsCommentById(model.Id);
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var previousIsApproved = comment.IsApproved;

            comment.IsApproved = model.IsApproved;
            _newsService.UpdateNews(comment.NewsItem);

            //activity log
            _customerActivityService.InsertActivity("EditNewsComment", _localizationService.GetResource("ActivityLog.EditNewsComment"), model.Id);

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

            var comment = _newsService.GetNewsCommentById(id);
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            _newsService.DeleteNewsComment(comment);

            //activity log
            _customerActivityService.InsertActivity("DeleteNewsComment", _localizationService.GetResource("ActivityLog.DeleteNewsComment"), id);

            return new NullJsonResult();
        }
        
        [HttpPost]
        public virtual IActionResult DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var comments = _newsService.GetNewsCommentsByIds(selectedIds.ToArray());

                _newsService.DeleteNewsComments(comments);

                //activity log
                foreach (var newsComment in comments)
                {
                    _customerActivityService.InsertActivity("DeleteNewsComment", _localizationService.GetResource("ActivityLog.DeleteNewsComment"), newsComment.Id);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ApproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                //filter not approved comments
                var newsComments = _newsService.GetNewsCommentsByIds(selectedIds.ToArray()).Where(comment => !comment.IsApproved);

                foreach (var newsComment in newsComments)
                {
                    newsComment.IsApproved = true;
                    _newsService.UpdateNews(newsComment.NewsItem);
                    
                    //raise event 
                    _eventPublisher.Publish(new NewsCommentApprovedEvent(newsComment));

                    //activity log
                    _customerActivityService.InsertActivity("EditNewsComment", _localizationService.GetResource("ActivityLog.EditNewsComment"), newsComment.Id);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                //filter approved comments
                var newsComments = _newsService.GetNewsCommentsByIds(selectedIds.ToArray()).Where(comment => comment.IsApproved);

                foreach (var newsComment in newsComments)
                {
                    newsComment.IsApproved = false;
                    _newsService.UpdateNews(newsComment.NewsItem);

                    //activity log
                    _customerActivityService.InsertActivity("EditNewsComment", _localizationService.GetResource("ActivityLog.EditNewsComment"), newsComment.Id);
                }
            }

            return Json(new { Result = true });
        }

        #endregion
    }
}