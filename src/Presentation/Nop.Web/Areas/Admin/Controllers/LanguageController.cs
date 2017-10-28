using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Core.Domain.Localization;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class LanguageController : BaseAdminController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrencyService _currencyService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IHostingEnvironment _hostingEnvironment;

        #endregion

        #region Ctor

        public LanguageController(ILanguageService languageService,
            ILocalizationService localizationService,
            ICurrencyService currencyService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            IHostingEnvironment hostingEnvironment)
        {
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._hostingEnvironment = hostingEnvironment;
        }

        #endregion

        #region Utilities
        
        protected virtual void PrepareStoresMappingModel(LanguageModel model, Language language, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && language != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(language).ToList();

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
        
        protected virtual void PrepareCurrenciesModel(LanguageModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //templates
            model.AvailableCurrencies.Add(new SelectListItem
            {
                Text = "---",
                Value = "0"
            });
            var currencies = _currencyService.GetAllCurrencies(true);
            foreach (var currency in currencies)
            {
                model.AvailableCurrencies.Add(new SelectListItem
                {
                    Text = currency.Name,
                    Value = currency.Id.ToString()
                });
            }
        }
        
        protected virtual void SaveStoreMappings(Language language, LanguageModel model)
        {
            language.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(language);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(language, store.Id);
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

        #region Languages

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedKendoGridJson();

            var languages = _languageService.GetAllLanguages(true, loadCacheableCopy: false);
            var gridModel = new DataSourceResult
            {
                Data = languages.Select(x => x.ToModel()),
                Total = languages.Count()
            };

            return Json(gridModel);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var model = new LanguageModel();
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //currencies
            PrepareCurrenciesModel(model);
            //default values
            model.Published = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var language = model.ToEntity();
                _languageService.InsertLanguage(language);

                //activity log
                _customerActivityService.InsertActivity("AddNewLanguage", _localizationService.GetResource("ActivityLog.AddNewLanguage"), language.Id);

                //Stores
                SaveStoreMappings(language, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = language.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, null, true);
            //currencies
            PrepareCurrenciesModel(model);

            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id, false);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");
            
            var model = language.ToModel();
            //Stores
            PrepareStoresMappingModel(model, language, false);
            //currencies
            PrepareCurrenciesModel(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(model.Id, false);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = _languageService.GetAllLanguages(loadCacheableCopy: false);
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id &&
                    !model.Published)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Configuration.Languages.PublishedLanguageRequired"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                _languageService.UpdateLanguage(language);

                //activity log
                _customerActivityService.InsertActivity("EditLanguage", _localizationService.GetResource("ActivityLog.EditLanguage"), language.Id);

                //Stores
                SaveStoreMappings(language, model);

                //notification
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = language.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, language, true);
            //currencies
            PrepareCurrenciesModel(model);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id, false);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = _languageService.GetAllLanguages(loadCacheableCopy: false);
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Configuration.Languages.PublishedLanguageRequired"));
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //delete
            _languageService.DeleteLanguage(language);

            //activity log
            _customerActivityService.InsertActivity("DeleteLanguage", _localizationService.GetResource("ActivityLog.DeleteLanguage"), language.Id);

            //notification
            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual JsonResult GetAvailableFlagFileNames()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return Json("Access denied");

            var flagNames = Directory
                .EnumerateFiles(Path.Combine(_hostingEnvironment.WebRootPath, "images\\flags"), "*.png", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .ToList();

            var availableFlagFileNames = flagNames.Select(flagName => new SelectListItem
            {
                Text = flagName,
                Value = flagName
            }).ToList();

            return Json(availableFlagFileNames);
        }

        #endregion

        #region Resources

        [HttpPost]
        public virtual IActionResult Resources(int languageId, DataSourceRequest command, LanguageResourcesListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedKendoGridJson();

            var query = _localizationService
                .GetAllResourceValues(languageId, null)
                .OrderBy(x => x.Key)
                .AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchResourceName))
                query = query.Where(l => l.Key.ToLowerInvariant().Contains(model.SearchResourceName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(model.SearchResourceValue))
                query = query.Where(l => l.Value.Value.ToLowerInvariant().Contains(model.SearchResourceValue.ToLowerInvariant()));

            var resources = query
                .Select(x => new LanguageResourceModel
                {
                    LanguageId = languageId,
                    Id = x.Value.Key,
                    Name = x.Key,
                    Value = x.Value.Value,
                });

            var gridModel = new DataSourceResult
            {
                Data = resources.PagedForCommand(command),
                Total = resources.Count()
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ResourceUpdate(LanguageResourceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var resource = _localizationService.GetLocaleStringResourceById(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = _localizationService.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    return Json(new DataSourceResult { Errors = string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName) });
                }
            }

            resource.ResourceName = model.Name;
            resource.ResourceValue = model.Value;
            _localizationService.UpdateLocaleStringResource(resource);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ResourceAdd(int languageId,LanguageResourceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var res = _localizationService.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
            if (res == null)
            {
                var resource = new LocaleStringResource { LanguageId = languageId };
                resource.ResourceName = model.Name;
                resource.ResourceValue = model.Value;
                _localizationService.InsertLocaleStringResource(resource);
            }
            else
            {
                return Json(new DataSourceResult { Errors = string.Format(_localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), model.Name) });
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ResourceDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var resource = _localizationService.GetLocaleStringResourceById(id);
            if (resource == null)
                throw new ArgumentException("No resource found with the specified id");
            _localizationService.DeleteLocaleStringResource(resource);

            return new NullJsonResult();
        }

        #endregion

        #region Export / Import

        public virtual IActionResult ExportXml(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id, false);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            try
            {
                var xml = _localizationService.ExportResourcesToXml(language);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "language_pack.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ImportXml(int id, IFormFile importxmlfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id, false);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            try
            {
                if (importxmlfile != null && importxmlfile.Length > 0)
                {
                    using (var sr = new StreamReader(importxmlfile.OpenReadStream(), Encoding.UTF8))
                    {
                        var content = sr.ReadToEnd();
                        _localizationService.ImportResourcesFromXml(language, content);
                    }

                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Imported"));
                return RedirectToAction("Edit", new { id = language.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }
        }

        #endregion
    }
}