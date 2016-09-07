using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.News;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.News;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.News;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class NewsController : BaseAdminController
	{
		#region Fields

        private readonly INewsService _newsService;
        private readonly ILanguageService _languageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        
		#endregion

		#region Constructors

        public NewsController(INewsService newsService, 
            ILanguageService languageService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            IStoreService storeService, 
            IStoreMappingService storeMappingService)
        {
            this._newsService = newsService;
            this._languageService = languageService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
		}

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareLanguagesModel(NewsItemModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

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

        [NonAction]
        protected virtual void PrepareStoresMappingModel(NewsItemModel model, NewsItem newsItem, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

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

        [NonAction]
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

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
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
        public ActionResult List(DataSourceRequest command, NewsItemListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var news = _newsService.GetAllNews(0, model.SearchStoreId, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = news.Select(x =>
                {
                    var m = x.ToModel();
                    //little hack here:
                    //ensure that descriptions are not returned
                    //otherwise, we can get the following error if entities have too long descriptions:
                    //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
                    //also it improves performance
                    m.Full = "";
                    if (x.StartDateUtc.HasValue)
                        m.StartDate = _dateTimeHelper.ConvertToUserTime(x.StartDateUtc.Value, DateTimeKind.Utc);
                    if (x.EndDateUtc.HasValue)
                        m.EndDate = _dateTimeHelper.ConvertToUserTime(x.EndDateUtc.Value, DateTimeKind.Utc);
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    m.LanguageName = x.Language.Name;
                    m.Comments = x.CommentCount;
                    return m;
                }),
                Total = news.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Create()
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
        public ActionResult Create(NewsItemModel model, bool continueEditing)
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

        public ActionResult Edit(int id)
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
        public ActionResult Edit(NewsItemModel model, bool continueEditing)
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
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var newsItem = _newsService.GetNewsById(id);
            if (newsItem == null)
                //No news item found with the specified id
                return RedirectToAction("List");

            _newsService.DeleteNews(newsItem);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Comments

        public ActionResult Comments(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            ViewBag.FilterByNewsItemId = filterByNewsItemId;
            return View();
        }

        [HttpPost]
        public ActionResult Comments(int? filterByNewsItemId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            IList<NewsComment> comments = filterByNewsItemId.HasValue ?
                //filter comments by news item
                _newsService.GetNewsById(filterByNewsItemId.Value).NewsComments.OrderBy(bc => bc.CreatedOnUtc).ToList() :
                //load all news comments
                _newsService.GetAllComments(0);

            var gridModel = new DataSourceResult
            {
                Data = comments.PagedForCommand(command).Select(newsComment =>
                {
                    var commentModel = new NewsCommentModel();
                    commentModel.Id = newsComment.Id;
                    commentModel.NewsItemId = newsComment.NewsItemId;
                    commentModel.NewsItemTitle = newsComment.NewsItem.Title;
                    commentModel.CustomerId = newsComment.CustomerId;
                    var customer = newsComment.Customer;
                    commentModel.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    commentModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(newsComment.CreatedOnUtc, DateTimeKind.Utc);
                    commentModel.CommentTitle = newsComment.CommentTitle;
                    commentModel.CommentText = Core.Html.HtmlHelper.FormatText(newsComment.CommentText, false, true, false, false, false, false);
                    return commentModel;
                }),
                Total = comments.Count,
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult CommentDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var comment = _newsService.GetNewsCommentById(id);
            if (comment == null)
                throw new ArgumentException("No comment found with the specified id");

            var newsItem = comment.NewsItem;
            _newsService.DeleteNewsComment(comment);
            //update totals
            newsItem.CommentCount = newsItem.NewsComments.Count;
            _newsService.UpdateNews(newsItem);

            return new NullJsonResult();
        }
        
        [HttpPost]
        public ActionResult DeleteSelectedComments(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var comments = _newsService.GetNewsCommentsByIds(selectedIds.ToArray());
                var news = _newsService.GetNewsByIds(comments.Select(p => p.NewsItemId).Distinct().ToArray());

                _newsService.DeleteNewsComments(comments);
                //update totals
                foreach (var newsItem in news)
                {
                    newsItem.CommentCount = newsItem.NewsComments.Count;
                    _newsService.UpdateNews(newsItem);
                }
            }

            return Json(new { Result = true });
        }

        #endregion
    }
}
