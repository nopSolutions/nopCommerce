using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Localization;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;

namespace Nop.Admin.Controllers
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

		#endregion

		#region Constructors

		public LanguageController(ILanguageService languageService,
            ILocalizationService localizationService,
            ICurrencyService currencyService,
            IStoreService storeService, 
            IStoreMappingService storeMappingService,
            IPermissionService permissionService)
		{
			this._localizationService = localizationService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
		}

		#endregion 

        #region Utilities

        [NonAction]
        protected virtual void PrepareFlagsModel(LanguageModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            
            model.FlagFileNames = Directory
                .EnumerateFiles(CommonHelper.MapPath("~/Content/Images/flags/"), "*.png", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .ToList();
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(LanguageModel model, Language language, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

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

        [NonAction]
        protected virtual void PrepareCurrenciesModel(LanguageModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

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

        [NonAction]
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

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

			return View();
		}

		[HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

			var languages = _languageService.GetAllLanguages(true);
			var gridModel = new DataSourceResult
			{
				Data = languages.Select(x => x.ToModel()),
				Total = languages.Count()
			};
			return new JsonResult
			{
				Data = gridModel
			};
		}
        
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var model = new LanguageModel();
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //currencies
            PrepareCurrenciesModel(model);
            //flags
            PrepareFlagsModel(model);
            //default values
            model.Published = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var language = model.ToEntity();
                _languageService.InsertLanguage(language);

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
            //flags
            PrepareFlagsModel(model);

            return View(model);
        }

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

			var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

		    var model = language.ToModel();
            //Stores
            PrepareStoresMappingModel(model, language, false);
            //currencies
            PrepareCurrenciesModel(model);
            //flags
            PrepareFlagsModel(model);

            return View(model);
		}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
		public ActionResult Edit(LanguageModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(model.Id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = _languageService.GetAllLanguages();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id &&
                    !model.Published)
                {
                    ErrorNotification("At least one published language is required.");
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                _languageService.UpdateLanguage(language);

                //Stores
                SaveStoreMappings(language, model);

                //notification
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = language.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, language, true);
            //currencies
            PrepareCurrenciesModel(model);
            //flags
            PrepareFlagsModel(model);

            return View(model);
		}

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = _languageService.GetAllLanguages();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
                ErrorNotification("At least one published language is required.");
                return RedirectToAction("Edit", new { id = language.Id });
            }
            
            //delete
            _languageService.DeleteLanguage(language);

            //notification
            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Languages.Deleted"));
            return RedirectToAction("List");
        }

		#endregion

		#region Resources
        
        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)] 
		public ActionResult Resources(int languageId, DataSourceRequest command, LanguageResourcesListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var query = _localizationService
                .GetAllResourceValues(languageId)
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
        public ActionResult ResourceUpdate(LanguageResourceModel model)
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
        public ActionResult ResourceAdd(int languageId, [Bind(Exclude = "Id")] LanguageResourceModel model)
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
        public ActionResult ResourceDelete(int id)
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

        public ActionResult ExportXml(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            try
            {
                var xml = _localizationService.ExportResourcesToXml(language);
                return new XmlDownloadResult(xml, "language_pack.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ImportXml(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                //No language found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            try
            {
                var file = Request.Files["importxmlfile"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var sr = new StreamReader(file.InputStream, Encoding.UTF8))
                    {
                        string content = sr.ReadToEnd();
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
