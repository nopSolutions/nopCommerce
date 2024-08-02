using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Shipping.Pickup;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ShippingController : BaseAdminController
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly ISettingService _settingService;
    protected readonly IShippingModelFactory _shippingModelFactory;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IShippingService _shippingService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IWorkContext _workContext;
    protected readonly ShippingSettings _shippingSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ShippingController(IAddressService addressService,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        IDateRangeService dateRangeService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IPickupPluginManager pickupPluginManager,
        ISettingService settingService,
        IShippingModelFactory shippingModelFactory,
        IShippingPluginManager shippingPluginManager,
        IShippingService shippingService,
        IGenericAttributeService genericAttributeService,
        IWorkContext workContext,
        ShippingSettings shippingSettings)
    {
        _addressService = addressService;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _dateRangeService = dateRangeService;
        _eventPublisher = eventPublisher;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _pickupPluginManager = pickupPluginManager;
        _settingService = settingService;
        _shippingModelFactory = shippingModelFactory;
        _shippingPluginManager = shippingPluginManager;
        _shippingService = shippingService;
        _genericAttributeService = genericAttributeService;
        _workContext = workContext;
        _shippingSettings = shippingSettings;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(ShippingMethod shippingMethod, ShippingMethodModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(shippingMethod, x => x.Name, localized.Name, localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(shippingMethod, x => x.Description, localized.Description, localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(DeliveryDate deliveryDate, DeliveryDateModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(deliveryDate, x => x.Name, localized.Name, localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(ProductAvailabilityRange productAvailabilityRange, ProductAvailabilityRangeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(productAvailabilityRange, x => x.Name, localized.Name, localized.LanguageId);
        }
    }

    #endregion

    #region Shipping rate computation methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Providers(bool showtour = false)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingProviderSearchModelAsync(new ShippingProviderSearchModel());

        //show configuration tour
        if (showtour)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if (!hideCard && !closeCard)
                ViewBag.ShowTour = true;
        }

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Providers(ShippingProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingProviderListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> ProviderUpdate(ShippingProviderModel model)
    {
        var srcm = await _shippingPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
        if (_shippingPluginManager.IsPluginActive(srcm))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(srcm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(srcm.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }

        var pluginDescriptor = srcm.PluginDescriptor;

        //display order
        pluginDescriptor.DisplayOrder = model.DisplayOrder;

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

        return new NullJsonResult();
    }

    #endregion

    #region Pickup point providers

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> PickupPointProviders()
    {
        //prepare model
        var model = await _shippingModelFactory.PreparePickupPointProviderSearchModelAsync(new PickupPointProviderSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> PickupPointProviders(PickupPointProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PreparePickupPointProviderListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> PickupPointProviderUpdate(PickupPointProviderModel model)
    {
        var pickupPointProvider = await _pickupPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
        if (_pickupPluginManager.IsPluginActive(pickupPointProvider))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _shippingSettings.ActivePickupPointProviderSystemNames.Remove(pickupPointProvider.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _shippingSettings.ActivePickupPointProviderSystemNames.Add(pickupPointProvider.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_shippingSettings);
            }
        }

        var pluginDescriptor = pickupPointProvider.PluginDescriptor;
        pluginDescriptor.DisplayOrder = model.DisplayOrder;

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

        return new NullJsonResult();
    }

    #endregion

    #region Shipping methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Methods()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodSearchModelAsync(new ShippingMethodSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Methods(ShippingMethodSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateMethod()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodModelAsync(new ShippingMethodModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateMethod(ShippingMethodModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var sm = model.ToEntity<ShippingMethod>();
            await _shippingService.InsertShippingMethodAsync(sm);

            //locales
            await UpdateLocalesAsync(sm, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Added"));
            return continueEditing ? RedirectToAction("EditMethod", new { id = sm.Id }) : RedirectToAction("Methods");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareShippingMethodModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditMethod(int id)
    {
        //try to get a shipping method with the specified id
        var shippingMethod = await _shippingService.GetShippingMethodByIdAsync(id);
        if (shippingMethod == null)
            return RedirectToAction("Methods");

        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodModelAsync(null, shippingMethod);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditMethod(ShippingMethodModel model, bool continueEditing)
    {
        //try to get a shipping method with the specified id
        var shippingMethod = await _shippingService.GetShippingMethodByIdAsync(model.Id);
        if (shippingMethod == null)
            return RedirectToAction("Methods");

        if (ModelState.IsValid)
        {
            shippingMethod = model.ToEntity(shippingMethod);
            await _shippingService.UpdateShippingMethodAsync(shippingMethod);

            //locales
            await UpdateLocalesAsync(shippingMethod, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Updated"));

            return continueEditing ? RedirectToAction("EditMethod", shippingMethod.Id) : RedirectToAction("Methods");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareShippingMethodModelAsync(model, shippingMethod, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteMethod(int id)
    {
        //try to get a shipping method with the specified id
        var shippingMethod = await _shippingService.GetShippingMethodByIdAsync(id);
        if (shippingMethod == null)
            return RedirectToAction("Methods");

        await _shippingService.DeleteShippingMethodAsync(shippingMethod);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Deleted"));

        return RedirectToAction("Methods");
    }

    #endregion

    #region Dates and ranges

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DatesAndRanges()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareDatesRangesSearchModelAsync(new DatesRangesSearchModel());

        return View(model);
    }

    #endregion

    #region Delivery dates

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeliveryDates(DeliveryDateSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareDeliveryDateListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateDeliveryDate()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(new DeliveryDateModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateDeliveryDate(DeliveryDateModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var deliveryDate = model.ToEntity<DeliveryDate>();
            await _dateRangeService.InsertDeliveryDateAsync(deliveryDate);

            //locales
            await UpdateLocalesAsync(deliveryDate, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Added"));

            return continueEditing ? RedirectToAction("EditDeliveryDate", new { id = deliveryDate.Id }) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditDeliveryDate(int id)
    {
        //try to get a delivery date with the specified id
        var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(id);
        if (deliveryDate == null)
            return RedirectToAction("DatesAndRanges");

        //prepare model
        var model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(null, deliveryDate);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditDeliveryDate(DeliveryDateModel model, bool continueEditing)
    {
        //try to get a delivery date with the specified id
        var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(model.Id);
        if (deliveryDate == null)
            return RedirectToAction("DatesAndRanges");

        if (ModelState.IsValid)
        {
            deliveryDate = model.ToEntity(deliveryDate);
            await _dateRangeService.UpdateDeliveryDateAsync(deliveryDate);

            //locales
            await UpdateLocalesAsync(deliveryDate, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Updated"));

            return continueEditing ? RedirectToAction("EditDeliveryDate", deliveryDate.Id) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareDeliveryDateModelAsync(model, deliveryDate, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteDeliveryDate(int id)
    {
        //try to get a delivery date with the specified id
        var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(id);
        if (deliveryDate == null)
            return RedirectToAction("DatesAndRanges");

        await _dateRangeService.DeleteDeliveryDateAsync(deliveryDate);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Deleted"));

        return RedirectToAction("DatesAndRanges");
    }

    #endregion

    #region Product availability ranges

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> ProductAvailabilityRanges(ProductAvailabilityRangeSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareProductAvailabilityRangeListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateProductAvailabilityRange()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(new ProductAvailabilityRangeModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var productAvailabilityRange = model.ToEntity<ProductAvailabilityRange>();
            await _dateRangeService.InsertProductAvailabilityRangeAsync(productAvailabilityRange);

            //locales
            await UpdateLocalesAsync(productAvailabilityRange, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Added"));

            return continueEditing ? RedirectToAction("EditProductAvailabilityRange", new { id = productAvailabilityRange.Id }) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditProductAvailabilityRange(int id)
    {
        //try to get a product availability range with the specified id
        var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(id);
        if (productAvailabilityRange == null)
            return RedirectToAction("DatesAndRanges");

        //prepare model
        var model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(null, productAvailabilityRange);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
    {
        //try to get a product availability range with the specified id
        var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(model.Id);
        if (productAvailabilityRange == null)
            return RedirectToAction("DatesAndRanges");

        if (ModelState.IsValid)
        {
            productAvailabilityRange = model.ToEntity(productAvailabilityRange);
            await _dateRangeService.UpdateProductAvailabilityRangeAsync(productAvailabilityRange);

            //locales
            await UpdateLocalesAsync(productAvailabilityRange, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Updated"));

            return continueEditing ? RedirectToAction("EditProductAvailabilityRange", productAvailabilityRange.Id) : RedirectToAction("DatesAndRanges");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareProductAvailabilityRangeModelAsync(model, productAvailabilityRange, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteProductAvailabilityRange(int id)
    {
        //try to get a product availability range with the specified id
        var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(id);
        if (productAvailabilityRange == null)
            return RedirectToAction("DatesAndRanges");

        await _dateRangeService.DeleteProductAvailabilityRangeAsync(productAvailabilityRange);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Deleted"));

        return RedirectToAction("DatesAndRanges");
    }

    #endregion

    #region Warehouses

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Warehouses()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseSearchModelAsync(new WarehouseSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Warehouses(WarehouseSearchModel searchModel)
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateWarehouse()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseModelAsync(new WarehouseModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> CreateWarehouse(WarehouseModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var address = model.Address.ToEntity<Address>();
            address.CreatedOnUtc = DateTime.UtcNow;
            await _addressService.InsertAddressAsync(address);

            //fill entity from model
            var warehouse = model.ToEntity<Warehouse>();
            warehouse.AddressId = address.Id;

            await _shippingService.InsertWarehouseAsync(warehouse);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewWarehouse",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewWarehouse"), warehouse.Id), warehouse);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Added"));

            return continueEditing ? RedirectToAction("EditWarehouse", new { id = warehouse.Id }) : RedirectToAction("Warehouses");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareWarehouseModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditWarehouse(int id)
    {
        //try to get a warehouse with the specified id
        var warehouse = await _shippingService.GetWarehouseByIdAsync(id);
        if (warehouse == null)
            return RedirectToAction("Warehouses");

        //prepare model
        var model = await _shippingModelFactory.PrepareWarehouseModelAsync(null, warehouse);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> EditWarehouse(WarehouseModel model, bool continueEditing)
    {
        //try to get a warehouse with the specified id
        var warehouse = await _shippingService.GetWarehouseByIdAsync(model.Id);
        if (warehouse == null)
            return RedirectToAction("Warehouses");

        if (ModelState.IsValid)
        {
            var address = await _addressService.GetAddressByIdAsync(warehouse.AddressId) ??
                          new Address
                          {
                              CreatedOnUtc = DateTime.UtcNow
                          };
            address = model.Address.ToEntity(address);
            if (address.Id > 0)
                await _addressService.UpdateAddressAsync(address);
            else
                await _addressService.InsertAddressAsync(address);

            //fill entity from model
            warehouse = model.ToEntity(warehouse);

            warehouse.AddressId = address.Id;

            await _shippingService.UpdateWarehouseAsync(warehouse);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditWarehouse",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditWarehouse"), warehouse.Id), warehouse);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Updated"));

            return continueEditing ? RedirectToAction("EditWarehouse", warehouse.Id) : RedirectToAction("Warehouses");
        }

        //prepare model
        model = await _shippingModelFactory.PrepareWarehouseModelAsync(model, warehouse, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DeleteWarehouse(int id)
    {
        //try to get a warehouse with the specified id
        var warehouse = await _shippingService.GetWarehouseByIdAsync(id);
        if (warehouse == null)
            return RedirectToAction("Warehouses");

        await _shippingService.DeleteWarehouseAsync(warehouse);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteWarehouse",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteWarehouse"), warehouse.Id), warehouse);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.warehouses.Deleted"));

        return RedirectToAction("Warehouses");
    }

    #endregion

    #region Restrictions

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Restrictions()
    {
        //prepare model
        var model = await _shippingModelFactory.PrepareShippingMethodRestrictionModelAsync(new ShippingMethodRestrictionModel());

        return View(model);
    }

    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("Restrictions")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> RestrictionSave(ShippingMethodRestrictionModel model, IFormCollection form)
    {
        var countries = await _countryService.GetAllCountriesAsync(showHidden: true);
        var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();

        foreach (var shippingMethod in shippingMethods)
        {
            var formKey = "restrict_" + shippingMethod.Id;
            var countryIdsToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                ? form[formKey].ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList()
                : new List<int>();

            foreach (var country in countries)
            {
                var restrict = countryIdsToRestrict.Contains(country.Id);
                var shippingMethodCountryMappings =
                    await _shippingService.GetShippingMethodCountryMappingAsync(shippingMethod.Id, country.Id);

                if (restrict)
                {
                    if (shippingMethodCountryMappings.Any())
                        continue;

                    await _shippingService.InsertShippingMethodCountryMappingAsync(new ShippingMethodCountryMapping { CountryId = country.Id, ShippingMethodId = shippingMethod.Id });
                    await _shippingService.UpdateShippingMethodAsync(shippingMethod);
                }
                else
                {
                    if (!shippingMethodCountryMappings.Any())
                        continue;

                    await _shippingService.DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMappings.FirstOrDefault());
                    await _shippingService.UpdateShippingMethodAsync(shippingMethod);
                }
            }
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

        return RedirectToAction("Restrictions");
    }

    #endregion
}