﻿using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
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

namespace Nop.Web.Areas.Admin.Controllers;

public partial class LanguageController : BaseAdminController
{
    #region Const

    protected const string FLAGS_PATH = "flags";

    #endregion

    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILanguageModelFactory _languageModelFactory;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public LanguageController(ICustomerActivityService customerActivityService,
        ILanguageModelFactory languageModelFactory,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INotificationService notificationService,
        IStoreMappingService storeMappingService,
        MediaSettings mediaSettings)
    {
        _customerActivityService = customerActivityService;
        _languageModelFactory = languageModelFactory;
        _languageService = languageService;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _notificationService = notificationService;
        _storeMappingService = storeMappingService;
        _mediaSettings = mediaSettings;
    }

    #endregion

    #region Languages

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _languageModelFactory.PrepareLanguageSearchModelAsync(new LanguageSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> List(LanguageSearchModel searchModel)
    {
        //prepare model
        var model = await _languageModelFactory.PrepareLanguageListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _languageModelFactory.PrepareLanguageModelAsync(new LanguageModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Create(LanguageModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var language = model.ToEntity<Language>();
            await _languageService.InsertLanguageAsync(language);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewLanguage",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewLanguage"), language.Id), language);

            //Stores
            await _storeMappingService.SaveStoreMappingsAsync(language, model.SelectedStoreIds);

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

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a language with the specified id
        var language = await _languageService.GetLanguageByIdAsync(id);
        if (language == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _languageModelFactory.PrepareLanguageModelAsync(null, language);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Edit(LanguageModel model, bool continueEditing)
    {
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
            await _storeMappingService.SaveStoreMappingsAsync(language, model.SelectedStoreIds);

            //notification
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.Updated"));
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Languages.NeedRestart"));

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
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Delete(int id)
    {
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
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual JsonResult GetAvailableFlagFileNames()
    {
        var flagNames = _fileProvider
            .EnumerateFiles(_fileProvider.Combine(_fileProvider.GetLocalImagesPath(_mediaSettings), FLAGS_PATH), "*.png")
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
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> Resources(LocaleResourceSearchModel searchModel)
    {
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
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ResourceUpdate([Validate] LocaleResourceModel model)
    {
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
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ResourceAdd(int languageId, [Validate] LocaleResourceModel model)
    {
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
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ResourceDelete(int id)
    {
        //try to get a locale resource with the specified id
        var resource = await _localizationService.GetLocaleStringResourceByIdAsync(id)
            ?? throw new ArgumentException("No resource found with the specified id", nameof(id));

        await _localizationService.DeleteLocaleStringResourceAsync(resource);

        return new NullJsonResult();
    }

    #endregion

    #region Export / Import

    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ExportXml(int id)
    {
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
    [CheckPermission(StandardPermission.Configuration.MANAGE_LANGUAGES)]
    public virtual async Task<IActionResult> ImportXml(int id, IFormFile importxmlfile)
    {
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