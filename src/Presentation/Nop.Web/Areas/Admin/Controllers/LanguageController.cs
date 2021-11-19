using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class LanguageController : BaseAdminController
    {
        #region Const

        private const string FLAGS_PATH = @"images\flags";

        #endregion

        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILanguageModelFactory _languageModelFactory;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public LanguageController(ICustomerActivityService customerActivityService,
            ILanguageModelFactory languageModelFactory,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            _customerActivityService = customerActivityService;
            _languageModelFactory = languageModelFactory;
            _languageService = languageService;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappingsAsync(Language language, LanguageModel model)
        {
            language.LimitedToStores = model.SelectedStoreIds.Any();
            await _languageService.UpdateLanguageAsync(language);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(language);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(language, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Languages

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageSearchModelAsync(new LanguageSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(LanguageSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageModelAsync(new LanguageModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(LanguageModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var language = model.ToEntity<Language>();
                await _languageService.InsertLanguageAsync(language);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewLanguage",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewLanguage"), language.Id), language);

                //Stores
                await SaveStoreMappingsAsync(language, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Added"));
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = language.Id });
            }

            //prepare model
            model = await _languageModelFactory.PrepareLanguageModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageModelAsync(null, language);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(LanguageModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(model.Id);
            if (language == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = await _languageService.GetAllLanguagesAsync();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id && !model.Published)
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.PublishedLanguageRequired"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                await _languageService.UpdateLanguageAsync(language);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditLanguage",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditLanguage"), language.Id), language);

                //Stores
                await SaveStoreMappingsAsync(language, model);

                //notification
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = language.Id });
            }

            //prepare model
            model = await _languageModelFactory.PrepareLanguageModelAsync(model, language, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = await _languageService.GetAllLanguagesAsync();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.PublishedLanguageRequired"));
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //delete
            await _languageService.DeleteLanguageAsync(language);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteLanguage",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteLanguage"), language.Id), language);

            //notification
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Deleted"));
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));
        
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<JsonResult> GetAvailableFlagFileNames()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return Json("Access denied");

            var flagNames = _fileProvider
                .EnumerateFiles(_fileProvider.GetAbsolutePath(FLAGS_PATH), "*.png")
                .Select(_fileProvider.GetFileName)
                .ToList();

            var availableFlagFileNames = flagNames.Select(flagName => new SelectListItem
            {
                Text = flagName,
                Value = flagName
            }).ToList();

            return Json(availableFlagFileNames);
        }

        //action displaying notification (warning) to a store owner that changed culture
        public virtual async Task<IActionResult> LanguageCultureWarning(string currentCulture, string changedCulture)
        {
            if (currentCulture != changedCulture)
            {
                return Json(new
                {
                    Result = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.CLDR.Warning"), 
                        Url.Action("GeneralCommon", "Setting"))
                });
            }

            return Json(new { Result = string.Empty });
        }

        #endregion

        #region Resources

        [HttpPost]
        public virtual async Task<IActionResult> Resources(LocaleResourceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return await AccessDeniedDataTablesJson();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(searchModel.LanguageId);
            if (language == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _languageModelFactory.PrepareLocaleResourceListModelAsync(searchModel, language);

            return Json(model);
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> ResourceUpdate([Validate] LocaleResourceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var resource = await _localizationService.GetLocaleStringResourceByIdAsync(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.ResourceName, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = await _localizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    return ErrorJson(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
                }
            }

            //fill entity from model
            resource = model.ToEntity(resource);

            await _localizationService.UpdateLocaleStringResourceAsync(resource);

            return new NullJsonResult();
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> ResourceAdd(int languageId, [Validate] LocaleResourceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var res = await _localizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId, false);
            if (res == null)
            {
                //fill entity from model
                var resource = model.ToEntity<LocaleStringResource>();

                resource.LanguageId = languageId;

                await _localizationService.InsertLocaleStringResourceAsync(resource);
            }
            else
            {
                return ErrorJson(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.NameAlreadyExists"), model.ResourceName));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ResourceDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a locale resource with the specified id
            var resource = await _localizationService.GetLocaleStringResourceByIdAsync(id)
                ?? throw new ArgumentException("No resource found with the specified id", nameof(id));

            await _localizationService.DeleteLocaleStringResourceAsync(resource);

            return new NullJsonResult();
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            try
            {
                var xml = await _localizationService.ExportResourcesToXmlAsync(language);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "language_pack.xml");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportXml(int id, IFormFile importxmlfile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            try
            {
                if (importxmlfile != null && importxmlfile.Length > 0)
                {
                    using var sr = new StreamReader(importxmlfile.OpenReadStream(), Encoding.UTF8);
                    await _localizationService.ImportResourcesFromXmlAsync(language, sr);
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Imported"));
                return RedirectToAction("Edit", new { id = language.Id });
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }
        }

        #endregion
    }
}