using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ContactFormAttributeController : BaseAdminController
{
    #region Fields

    protected readonly IAttributeService<ContactFormAttribute, ContactFormAttributeValue> _contactFormAttributeService;
    protected readonly IContactFormAttributeModelFactory _contactFormAttributeModelFactory;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public ContactFormAttributeController(
        IAttributeService<ContactFormAttribute, ContactFormAttributeValue> contactFormAttributeService,
        ICustomerActivityService customerActivityService,
        IContactFormAttributeModelFactory contactFormAttributeModelFactory,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _contactFormAttributeService = contactFormAttributeService;
        _customerActivityService = customerActivityService;
        _contactFormAttributeModelFactory = contactFormAttributeModelFactory;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateAttributeLocalesAsync(ContactFormAttribute contactFormAttribute, ContactFormAttributeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(contactFormAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateValueLocalesAsync(ContactFormAttributeValue contactFormAttributeValue, ContactFormAttributeValueModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(contactFormAttributeValue,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    #endregion

    #region Contact form attributes

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual IActionResult List()
    {
        //select an appropriate card
        SaveSelectedCardName("generalcommon-contactform");

        //we just redirect a user to the General settings page
        return RedirectToAction("GeneralCommon", "Setting");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> List(ContactFormAttributeSearchModel searchModel)
    {
        //prepare model
        var model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeModelAsync(new ContactFormAttributeModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create(ContactFormAttributeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var contactFormAttribute = model.ToEntity<ContactFormAttribute>();
            await _contactFormAttributeService.InsertAttributeAsync(contactFormAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewContactFormAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewContactFormAttribute"), contactFormAttribute.Id),
                contactFormAttribute);

            //locales
            await UpdateAttributeLocalesAsync(contactFormAttribute, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.ContactFormAttributes.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = contactFormAttribute.Id });
        }

        //prepare model
        model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(id);
        if (contactFormAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeModelAsync(null, contactFormAttribute);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(ContactFormAttributeModel model, bool continueEditing)
    {
        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(model.Id);

        if (contactFormAttribute == null)
            return RedirectToAction("List");

        if (!ModelState.IsValid)
            return View(model);

        contactFormAttribute = model.ToEntity(contactFormAttribute);
        await _contactFormAttributeService.UpdateAttributeAsync(contactFormAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditContactFormAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditContactFormAttribute"), contactFormAttribute.Id),
            contactFormAttribute);

        //locales
        await UpdateAttributeLocalesAsync(contactFormAttribute, model);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.ContactFormAttributes.Updated"));

        if (!continueEditing)
            return RedirectToAction("List");

        return RedirectToAction("Edit", new { id = contactFormAttribute.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(id);
        await _contactFormAttributeService.DeleteAttributeAsync(contactFormAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteContactFormAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteContactFormAttribute"), contactFormAttribute.Id),
            contactFormAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Common.ContactFormAttributes.Deleted"));
        return RedirectToAction("List");
    }

    #endregion

    #region Contact form attribute values

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueList(ContactFormAttributeValueSearchModel searchModel)
    {
        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(searchModel.ContactFormAttributeId)
            ?? throw new ArgumentException("No contact form attribute found with the specified id");

        //prepare model
        var model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeValueListModelAsync(searchModel, contactFormAttribute);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(int contactFormAttributeId)
    {
        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(contactFormAttributeId);
        if (contactFormAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _contactFormAttributeModelFactory
            .PrepareContactFormAttributeValueModelAsync(new ContactFormAttributeValueModel(), contactFormAttribute, null);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(ContactFormAttributeValueModel model)
    {
        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(model.AttributeId);
        if (contactFormAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var cfav = model.ToEntity<ContactFormAttributeValue>();
            await _contactFormAttributeService.InsertAttributeValueAsync(cfav);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewContactFormAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewContactFormAttributeValue"), cfav.Id), cfav);

            await UpdateValueLocalesAsync(cfav, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeValueModelAsync(model, contactFormAttribute, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(int id)
    {
        var contactFormAttributeValue = await _contactFormAttributeService.GetAttributeValueByIdAsync(id);
        if (contactFormAttributeValue == null)
            return RedirectToAction("List");

        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(contactFormAttributeValue.AttributeId);
        if (contactFormAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeValueModelAsync(null, contactFormAttribute, contactFormAttributeValue);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(ContactFormAttributeValueModel model)
    {
        var contactFormAttributeValue = await _contactFormAttributeService.GetAttributeValueByIdAsync(model.Id);
        if (contactFormAttributeValue == null)
            return RedirectToAction("List");

        var contactFormAttribute = await _contactFormAttributeService.GetAttributeByIdAsync(contactFormAttributeValue.AttributeId);
        if (contactFormAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            contactFormAttributeValue = model.ToEntity(contactFormAttributeValue);
            await _contactFormAttributeService.UpdateAttributeValueAsync(contactFormAttributeValue);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditContactFormAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditContactFormAttributeValue"), contactFormAttributeValue.Id),
                contactFormAttributeValue);

            await UpdateValueLocalesAsync(contactFormAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _contactFormAttributeModelFactory.PrepareContactFormAttributeValueModelAsync(model, contactFormAttribute, contactFormAttributeValue, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueDelete(int id)
    {
        var contactFormAttributeValue = await _contactFormAttributeService.GetAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No contact form attribute value found with the specified id", nameof(id));

        await _contactFormAttributeService.DeleteAttributeValueAsync(contactFormAttributeValue);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteContactFormAttributeValue",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteContactFormAttributeValue"), contactFormAttributeValue.Id),
            contactFormAttributeValue);

        return new NullJsonResult();
    }

    #endregion
}