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

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILanguageModelFactory LanguageModelFactory { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INopFileProvider FileProvider { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IStoreService StoreService { get; }

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
            CustomerActivityService = customerActivityService;
            LanguageModelFactory = languageModelFactory;
            LanguageService = languageService;
            LocalizationService = localizationService;
            FileProvider = fileProvider;
            NotificationService = notificationService;
            PermissionService = permissionService;
            StoreMappingService = storeMappingService;
            StoreService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task SaveStoreMappingsAsync(Language language, LanguageModel model)
        {
            language.LimitedToStores = model.SelectedStoreIds.Any();
            await LanguageService.UpdateLanguageAsync(language);

            var existingStoreMappings = await StoreMappingService.GetStoreMappingsAsync(language);
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await StoreMappingService.InsertStoreMappingAsync(language, store.Id);
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

        #region Languages

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //prepare model
            var model = await LanguageModelFactory.PrepareLanguageSearchModelAsync(new LanguageSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(LanguageSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await LanguageModelFactory.PrepareLanguageListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //prepare model
            var model = await LanguageModelFactory.PrepareLanguageModelAsync(new LanguageModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(LanguageModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var language = model.ToEntity<Language>();
                await LanguageService.InsertLanguageAsync(language);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewLanguage",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewLanguage"), language.Id), language);

                //Stores
                await SaveStoreMappingsAsync(language, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.Added"));
                NotificationService.WarningNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = language.Id });
            }

            //prepare model
            model = await LanguageModelFactory.PrepareLanguageModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await LanguageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            //prepare model
            var model = await LanguageModelFactory.PrepareLanguageModelAsync(null, language);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(LanguageModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await LanguageService.GetLanguageByIdAsync(model.Id);
            if (language == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = await LanguageService.GetAllLanguagesAsync();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id && !model.Published)
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.PublishedLanguageRequired"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                await LanguageService.UpdateLanguageAsync(language);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditLanguage",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditLanguage"), language.Id), language);

                //Stores
                await SaveStoreMappingsAsync(language, model);

                //notification
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = language.Id });
            }

            //prepare model
            model = await LanguageModelFactory.PrepareLanguageModelAsync(model, language, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await LanguageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = await LanguageService.GetAllLanguagesAsync();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.PublishedLanguageRequired"));
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //delete
            await LanguageService.DeleteLanguageAsync(language);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteLanguage",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteLanguage"), language.Id), language);

            //notification
            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.Deleted"));
            NotificationService.WarningNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));
        
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<JsonResult> GetAvailableFlagFileNames()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return Json("Access denied");

            var flagNames = FileProvider
                .EnumerateFiles(FileProvider.GetAbsolutePath(FLAGS_PATH), "*.png")
                .Select(FileProvider.GetFileName)
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
                    Result = string.Format(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.CLDR.Warning"), 
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return await AccessDeniedDataTablesJson();

            //try to get a language with the specified id
            var language = await LanguageService.GetLanguageByIdAsync(searchModel.LanguageId);
            if (language == null)
                return RedirectToAction("List");

            //prepare model
            var model = await LanguageModelFactory.PrepareLocaleResourceListModelAsync(searchModel, language);

            return Json(model);
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> ResourceUpdate([Validate] LocaleResourceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var resource = await LocalizationService.GetLocaleStringResourceByIdAsync(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.ResourceName, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = await LocalizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    return ErrorJson(string.Format(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.NameAlreadyExists"), res.ResourceName));
                }
            }

            //fill entity from model
            resource = model.ToEntity(resource);

            await LocalizationService.UpdateLocaleStringResourceAsync(resource);

            return new NullJsonResult();
        }

        //ValidateAttribute is used to force model validation
        [HttpPost]
        public virtual async Task<IActionResult> ResourceAdd(int languageId, [Validate] LocaleResourceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            if (model.ResourceName != null)
                model.ResourceName = model.ResourceName.Trim();
            if (model.ResourceValue != null)
                model.ResourceValue = model.ResourceValue.Trim();

            if (!ModelState.IsValid)
            {
                return ErrorJson(ModelState.SerializeErrors());
            }

            var res = await LocalizationService.GetLocaleStringResourceByNameAsync(model.ResourceName, model.LanguageId, false);
            if (res == null)
            {
                //fill entity from model
                var resource = model.ToEntity<LocaleStringResource>();

                resource.LanguageId = languageId;

                await LocalizationService.InsertLocaleStringResourceAsync(resource);
            }
            else
            {
                return ErrorJson(string.Format(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.NameAlreadyExists"), model.ResourceName));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ResourceDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a locale resource with the specified id
            var resource = await LocalizationService.GetLocaleStringResourceByIdAsync(id)
                ?? throw new ArgumentException("No resource found with the specified id", nameof(id));

            await LocalizationService.DeleteLocaleStringResourceAsync(resource);

            return new NullJsonResult();
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await LanguageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            try
            {
                var xml = await LocalizationService.ExportResourcesToXmlAsync(language);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "language_pack.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportXml(int id, IFormFile importxmlfile)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            //try to get a language with the specified id
            var language = await LanguageService.GetLanguageByIdAsync(id);
            if (language == null)
                return RedirectToAction("List");

            try
            {
                if (importxmlfile != null && importxmlfile.Length > 0)
                {
                    using var sr = new StreamReader(importxmlfile.OpenReadStream(), Encoding.UTF8);
                    await LocalizationService.ImportResourcesFromXmlAsync(language, sr);
                }
                else
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Languages.Imported"));
                return RedirectToAction("Edit", new { id = language.Id });
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = language.Id });
            }
        }

        #endregion
    }
}