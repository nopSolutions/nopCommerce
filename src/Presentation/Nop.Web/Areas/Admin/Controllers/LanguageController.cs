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

        protected virtual async Task SaveStoreMappings(Language language, LanguageModel model)
        {
            language.LimitedToStores = model.SelectedStoreIds.Any();
            await _languageService.UpdateLanguage(language);

            var existingStoreMappings = await _storeMappingService.GetStoreMappings(language);
            var allStores = await _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        await _storeMappingService.InsertStoreMapping(language, store.Id);
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

        #region Languages

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageSearchModel(new LanguageSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(LanguageSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageListModel(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageModel(new LanguageModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(LanguageModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var language = model.ToEntity<Language>();
                await _languageService.InsertLanguage(language);

                //activity log
                await _customerActivityService.InsertActivity("AddNewLanguage",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewLanguage"), language.Id), language);

                //Stores
                await SaveStoreMappings(language, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Languages.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = language.Id });
            }

            //prepare model
            model = await _languageModelFactory.PrepareLanguageModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageById(id);
            if (language == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageModel(null, language);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(LanguageModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageById(model.Id);
            if (language == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = await _languageService.GetAllLanguages();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id && !model.Published)
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Configuration.Languages.PublishedLanguageRequired"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                await _languageService.UpdateLanguage(language);

                //activity log
                await _customerActivityService.InsertActivity("EditLanguage",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditLanguage"), language.Id), language);

                //Stores
                await SaveStoreMappings(language, model);

                //notification
                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Languages.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = language.Id });
            }

            //prepare model
            model = await _languageModelFactory.PrepareLanguageModel(model, language, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageById(id);
            if (language == null)
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = await _languageService.GetAllLanguages();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Configuration.Languages.PublishedLanguageRequired"));
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //delete
            await _languageService.DeleteLanguage(language);

            //activity log
            await _customerActivityService.InsertActivity("DeleteLanguage",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteLanguage"), language.Id), language);

            //notification
            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Languages.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<JsonResult> GetAvailableFlagFileNames()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
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

        #endregion

        #region Resources

        [HttpPost]
        public virtual async Task<IActionResult> Resources(LocaleResourceSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedDataTablesJson();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageById(searchModel.LanguageId);
            if (language == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _languageModelFactory.PrepareLocaleResourceListModel(searchModel, language);

            return Json(model);
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> ResourceUpdate([Validate] LocaleResourceModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var resource = await _localizationService.GetLocaleStringResourceById(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.ResourceName, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = await _localizationService.GetLocaleStringResourceByName(model.ResourceName, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    return ErrorJson(string.Format(await _localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
                }
            }

            //fill entity from model
            resource = model.ToEntity(resource);

            await _localizationService.UpdateLocaleStringResource(resource);

            return new NullJsonResult();
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> ResourceAdd(int languageId, [Validate] LocaleResourceModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var res = _localizationService.GetLocaleStringResourceByName(model.ResourceName, model.LanguageId, false);
            if (res == null)
            {
                //fill entity from model
                var resource = model.ToEntity<LocaleStringResource>();

                resource.LanguageId = languageId;

                await _localizationService.InsertLocaleStringResource(resource);
            }
            else
            {
                return ErrorJson(string.Format(await _localizationService.GetResource("Admin.Configuration.Languages.Resources.NameAlreadyExists"), model.ResourceName));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ResourceDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a locale resource with the specified id
            var resource = await _localizationService.GetLocaleStringResourceById(id)
                ?? throw new ArgumentException("No resource found with the specified id", nameof(id));

            await _localizationService.DeleteLocaleStringResource(resource);

            return new NullJsonResult();
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageById(id);
            if (language == null)
                return RedirectToAction("List");

            try
            {
                var xml = await _localizationService.ExportResourcesToXml(language);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "language_pack.xml");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportXml(int id, IFormFile importxmlfile)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await _languageService.GetLanguageById(id);
            if (language == null)
                return RedirectToAction("List");

            try
            {
                if (importxmlfile != null && importxmlfile.Length > 0)
                {
                    using var sr = new StreamReader(importxmlfile.OpenReadStream(), Encoding.UTF8);
                    await _localizationService.ImportResourcesFromXml(language, sr);
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Configuration.Languages.Imported"));
                return RedirectToAction("Edit", new { id = language.Id });
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }
        }

        #endregion
    }
}