using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Attributes;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class CustomerAttributeController : BaseAdminController
{
    #region Fields

    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerAttributeModelFactory _customerAttributeModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public CustomerAttributeController(IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        ICustomerActivityService customerActivityService,
        ICustomerAttributeModelFactory customerAttributeModelFactory,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService)
    {
        _customerAttributeService = customerAttributeService;
        _customerActivityService = customerActivityService;
        _customerAttributeModelFactory = customerAttributeModelFactory;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateAttributeLocalesAsync(CustomerAttribute customerAttribute, CustomerAttributeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(customerAttribute,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateValueLocalesAsync(CustomerAttributeValue customerAttributeValue, CustomerAttributeValueModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(customerAttributeValue,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    #endregion

    #region Customer attributes

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual IActionResult List()
    {
        //select an appropriate card
        SaveSelectedCardName("customersettings-customerformfields");

        //we just redirect a user to the customer settings page
        return RedirectToAction("CustomerUser", "Setting");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> List(CustomerAttributeSearchModel searchModel)
    {
        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeModelAsync(new CustomerAttributeModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Create(CustomerAttributeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var customerAttribute = model.ToEntity<CustomerAttribute>();
            await _customerAttributeService.InsertAttributeAsync(customerAttribute);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCustomerAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCustomerAttribute"), customerAttribute.Id),
                customerAttribute);

            //locales
            await UpdateAttributeLocalesAsync(customerAttribute, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = customerAttribute.Id });
        }

        //prepare model
        model = await _customerAttributeModelFactory.PrepareCustomerAttributeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(id);
        if (customerAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeModelAsync(null, customerAttribute);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Edit(CustomerAttributeModel model, bool continueEditing)
    {
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(model.Id);
        if (customerAttribute == null)
            //no customer attribute found with the specified id
            return RedirectToAction("List");

        if (!ModelState.IsValid)
            //if we got this far, something failed, redisplay form
            return View(model);

        customerAttribute = model.ToEntity(customerAttribute);
        await _customerAttributeService.UpdateAttributeAsync(customerAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditCustomerAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCustomerAttribute"), customerAttribute.Id),
            customerAttribute);

        //locales
        await UpdateAttributeLocalesAsync(customerAttribute, model);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Updated"));

        if (!continueEditing)
            return RedirectToAction("List");

        return RedirectToAction("Edit", new { id = customerAttribute.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(id);
        await _customerAttributeService.DeleteAttributeAsync(customerAttribute);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCustomerAttribute",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomerAttribute"), customerAttribute.Id),
            customerAttribute);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.CustomerAttributes.Deleted"));
        return RedirectToAction("List");
    }

    #endregion

    #region Customer attribute values

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueList(CustomerAttributeValueSearchModel searchModel)
    {
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(searchModel.CustomerAttributeId)
            ?? throw new ArgumentException("No customer attribute found with the specified id");

        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueListModelAsync(searchModel, customerAttribute);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(int customerAttributeId)
    {
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(customerAttributeId);
        if (customerAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerAttributeModelFactory
            .PrepareCustomerAttributeValueModelAsync(new CustomerAttributeValueModel(), customerAttribute, null);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueCreatePopup(CustomerAttributeValueModel model)
    {
        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(model.AttributeId);
        if (customerAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var cav = model.ToEntity<CustomerAttributeValue>();
            await _customerAttributeService.InsertAttributeValueAsync(cav);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewCustomerAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCustomerAttributeValue"), cav.Id), cav);

            await UpdateValueLocalesAsync(cav, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(model, customerAttribute, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(int id)
    {
        //try to get a customer attribute value with the specified id
        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(id);
        if (customerAttributeValue == null)
            return RedirectToAction("List");

        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(customerAttributeValue.AttributeId);
        if (customerAttribute == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(null, customerAttribute, customerAttributeValue);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueEditPopup(CustomerAttributeValueModel model)
    {
        //try to get a customer attribute value with the specified id
        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(model.Id);
        if (customerAttributeValue == null)
            return RedirectToAction("List");

        //try to get a customer attribute with the specified id
        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(customerAttributeValue.AttributeId);
        if (customerAttribute == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            customerAttributeValue = model.ToEntity(customerAttributeValue);
            await _customerAttributeService.UpdateAttributeValueAsync(customerAttributeValue);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditCustomerAttributeValue",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCustomerAttributeValue"), customerAttributeValue.Id),
                customerAttributeValue);

            await UpdateValueLocalesAsync(customerAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _customerAttributeModelFactory.PrepareCustomerAttributeValueModelAsync(model, customerAttribute, customerAttributeValue, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ValueDelete(int id)
    {
        //try to get a customer attribute value with the specified id
        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No customer attribute value found with the specified id", nameof(id));

        await _customerAttributeService.DeleteAttributeValueAsync(customerAttributeValue);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteCustomerAttributeValue",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCustomerAttributeValue"), customerAttributeValue.Id),
            customerAttributeValue);

        return new NullJsonResult();
    }

    #endregion
}