using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Events;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ShippingController : BaseAdminController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPickupPluginManager _pickupPluginManager;
        private readonly ISettingService _settingService;
        private readonly IShippingModelFactory _shippingModelFactory;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShippingService _shippingService;
        private readonly ShippingSettings _shippingSettings;

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
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(ShippingMethod shippingMethod, ShippingMethodModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(shippingMethod, x => x.Name, localized.Name, localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(shippingMethod, x => x.Description, localized.Description, localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(DeliveryDate deliveryDate, DeliveryDateModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(deliveryDate, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ProductAvailabilityRange productAvailabilityRange, ProductAvailabilityRangeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productAvailabilityRange, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        #endregion

        #region Shipping rate computation methods

        public virtual IActionResult Providers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareShippingProviderSearchModel(new ShippingProviderSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Providers(ShippingProviderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _shippingModelFactory.PrepareShippingProviderListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProviderUpdate(ShippingProviderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var srcm = _shippingPluginManager.LoadPluginBySystemName(model.SystemName);
            if (_shippingPluginManager.IsPluginActive(srcm))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(srcm.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(srcm.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }

            var pluginDescriptor = srcm.PluginDescriptor;

            //display order
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            _eventPublisher.Publish(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        #endregion

        #region Pickup point providers

        public virtual IActionResult PickupPointProviders()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PreparePickupPointProviderSearchModel(new PickupPointProviderSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult PickupPointProviders(PickupPointProviderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _shippingModelFactory.PreparePickupPointProviderListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult PickupPointProviderUpdate(PickupPointProviderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPointProvider = _pickupPluginManager.LoadPluginBySystemName(model.SystemName);
            if (_pickupPluginManager.IsPluginActive(pickupPointProvider))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _shippingSettings.ActivePickupPointProviderSystemNames.Remove(pickupPointProvider.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _shippingSettings.ActivePickupPointProviderSystemNames.Add(pickupPointProvider.PluginDescriptor.SystemName);
                    _settingService.SaveSetting(_shippingSettings);
                }
            }

            var pluginDescriptor = pickupPointProvider.PluginDescriptor;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            _eventPublisher.Publish(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        #endregion

        #region Shipping methods

        public virtual IActionResult Methods()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareShippingMethodSearchModel(new ShippingMethodSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Methods(ShippingMethodSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _shippingModelFactory.PrepareShippingMethodListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult CreateMethod()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareShippingMethodModel(new ShippingMethodModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateMethod(ShippingMethodModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var sm = model.ToEntity<ShippingMethod>();
                _shippingService.InsertShippingMethod(sm);

                //locales
                UpdateLocales(sm, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Methods.Added"));
                return continueEditing ? RedirectToAction("EditMethod", new { id = sm.Id }) : RedirectToAction("Methods");
            }

            //prepare model
            model = _shippingModelFactory.PrepareShippingMethodModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditMethod(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a shipping method with the specified id
            var shippingMethod = _shippingService.GetShippingMethodById(id);
            if (shippingMethod == null)
                return RedirectToAction("Methods");

            //prepare model
            var model = _shippingModelFactory.PrepareShippingMethodModel(null, shippingMethod);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditMethod(ShippingMethodModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a shipping method with the specified id
            var shippingMethod = _shippingService.GetShippingMethodById(model.Id);
            if (shippingMethod == null)
                return RedirectToAction("Methods");

            if (ModelState.IsValid)
            {
                shippingMethod = model.ToEntity(shippingMethod);
                _shippingService.UpdateShippingMethod(shippingMethod);

                //locales
                UpdateLocales(shippingMethod, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Methods.Updated"));

                return continueEditing ? RedirectToAction("EditMethod", shippingMethod.Id) : RedirectToAction("Methods");
            }

            //prepare model
            model = _shippingModelFactory.PrepareShippingMethodModel(model, shippingMethod, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteMethod(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a shipping method with the specified id
            var shippingMethod = _shippingService.GetShippingMethodById(id);
            if (shippingMethod == null)
                return RedirectToAction("Methods");

            _shippingService.DeleteShippingMethod(shippingMethod);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Methods.Deleted"));

            return RedirectToAction("Methods");
        }

        #endregion

        #region Dates and ranges

        public virtual IActionResult DatesAndRanges()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareDatesRangesSearchModel(new DatesRangesSearchModel());

            return View(model);
        }

        #endregion

        #region Delivery dates

        [HttpPost]
        public virtual IActionResult DeliveryDates(DeliveryDateSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _shippingModelFactory.PrepareDeliveryDateListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult CreateDeliveryDate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareDeliveryDateModel(new DeliveryDateModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateDeliveryDate(DeliveryDateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var deliveryDate = model.ToEntity<DeliveryDate>();
                _dateRangeService.InsertDeliveryDate(deliveryDate);

                //locales
                UpdateLocales(deliveryDate, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.DeliveryDates.Added"));

                return continueEditing ? RedirectToAction("EditDeliveryDate", new { id = deliveryDate.Id }) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = _shippingModelFactory.PrepareDeliveryDateModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditDeliveryDate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a delivery date with the specified id
            var deliveryDate = _dateRangeService.GetDeliveryDateById(id);
            if (deliveryDate == null)
                return RedirectToAction("DatesAndRanges");

            //prepare model
            var model = _shippingModelFactory.PrepareDeliveryDateModel(null, deliveryDate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditDeliveryDate(DeliveryDateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a delivery date with the specified id
            var deliveryDate = _dateRangeService.GetDeliveryDateById(model.Id);
            if (deliveryDate == null)
                return RedirectToAction("DatesAndRanges");

            if (ModelState.IsValid)
            {
                deliveryDate = model.ToEntity(deliveryDate);
                _dateRangeService.UpdateDeliveryDate(deliveryDate);

                //locales
                UpdateLocales(deliveryDate, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.DeliveryDates.Updated"));

                return continueEditing ? RedirectToAction("EditDeliveryDate", deliveryDate.Id) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = _shippingModelFactory.PrepareDeliveryDateModel(model, deliveryDate, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteDeliveryDate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a delivery date with the specified id
            var deliveryDate = _dateRangeService.GetDeliveryDateById(id);
            if (deliveryDate == null)
                return RedirectToAction("DatesAndRanges");

            _dateRangeService.DeleteDeliveryDate(deliveryDate);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.DeliveryDates.Deleted"));

            return RedirectToAction("DatesAndRanges");
        }

        #endregion

        #region Product availability ranges

        [HttpPost]
        public virtual IActionResult ProductAvailabilityRanges(ProductAvailabilityRangeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _shippingModelFactory.PrepareProductAvailabilityRangeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult CreateProductAvailabilityRange()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareProductAvailabilityRangeModel(new ProductAvailabilityRangeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAvailabilityRange = model.ToEntity<ProductAvailabilityRange>();
                _dateRangeService.InsertProductAvailabilityRange(productAvailabilityRange);

                //locales
                UpdateLocales(productAvailabilityRange, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.ProductAvailabilityRanges.Added"));

                return continueEditing ? RedirectToAction("EditProductAvailabilityRange", new { id = productAvailabilityRange.Id }) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = _shippingModelFactory.PrepareProductAvailabilityRangeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditProductAvailabilityRange(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a product availability range with the specified id
            var productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(id);
            if (productAvailabilityRange == null)
                return RedirectToAction("DatesAndRanges");

            //prepare model
            var model = _shippingModelFactory.PrepareProductAvailabilityRangeModel(null, productAvailabilityRange);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a product availability range with the specified id
            var productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(model.Id);
            if (productAvailabilityRange == null)
                return RedirectToAction("DatesAndRanges");

            if (ModelState.IsValid)
            {
                productAvailabilityRange = model.ToEntity(productAvailabilityRange);
                _dateRangeService.UpdateProductAvailabilityRange(productAvailabilityRange);

                //locales
                UpdateLocales(productAvailabilityRange, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.ProductAvailabilityRanges.Updated"));

                return continueEditing ? RedirectToAction("EditProductAvailabilityRange", productAvailabilityRange.Id) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = _shippingModelFactory.PrepareProductAvailabilityRangeModel(model, productAvailabilityRange, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteProductAvailabilityRange(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a product availability range with the specified id
            var productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(id);
            if (productAvailabilityRange == null)
                return RedirectToAction("DatesAndRanges");

            _dateRangeService.DeleteProductAvailabilityRange(productAvailabilityRange);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.ProductAvailabilityRanges.Deleted"));

            return RedirectToAction("DatesAndRanges");
        }

        #endregion

        #region Warehouses

        public virtual IActionResult Warehouses()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareWarehouseSearchModel(new WarehouseSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Warehouses(WarehouseSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _shippingModelFactory.PrepareWarehouseListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult CreateWarehouse()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareWarehouseModel(new WarehouseModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateWarehouse(WarehouseModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity<Address>();
                address.CreatedOnUtc = DateTime.UtcNow;
                _addressService.InsertAddress(address);

                //fill entity from model
                var warehouse = model.ToEntity<Warehouse>();
                warehouse.AddressId = address.Id;

                _shippingService.InsertWarehouse(warehouse);

                //activity log
                _customerActivityService.InsertActivity("AddNewWarehouse",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewWarehouse"), warehouse.Id), warehouse);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Added"));

                return continueEditing ? RedirectToAction("EditWarehouse", new { id = warehouse.Id }) : RedirectToAction("Warehouses");
            }

            //prepare model
            model = _shippingModelFactory.PrepareWarehouseModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditWarehouse(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a warehouse with the specified id
            var warehouse = _shippingService.GetWarehouseById(id);
            if (warehouse == null)
                return RedirectToAction("Warehouses");

            //prepare model
            var model = _shippingModelFactory.PrepareWarehouseModel(null, warehouse);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditWarehouse(WarehouseModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a warehouse with the specified id
            var warehouse = _shippingService.GetWarehouseById(model.Id);
            if (warehouse == null)
                return RedirectToAction("Warehouses");

            if (ModelState.IsValid)
            {
                var address = _addressService.GetAddressById(warehouse.AddressId) ??
                    new Address
                    {
                        CreatedOnUtc = DateTime.UtcNow
                    };
                address = model.Address.ToEntity(address);
                if (address.Id > 0)
                    _addressService.UpdateAddress(address);
                else
                    _addressService.InsertAddress(address);

                //fill entity from model
                warehouse = model.ToEntity(warehouse);

                warehouse.AddressId = address.Id;

                _shippingService.UpdateWarehouse(warehouse);

                //activity log
                _customerActivityService.InsertActivity("EditWarehouse",
                    string.Format(_localizationService.GetResource("ActivityLog.EditWarehouse"), warehouse.Id), warehouse);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Updated"));

                return continueEditing ? RedirectToAction("EditWarehouse", warehouse.Id) : RedirectToAction("Warehouses");
            }

            //prepare model
            model = _shippingModelFactory.PrepareWarehouseModel(model, warehouse, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteWarehouse(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a warehouse with the specified id
            var warehouse = _shippingService.GetWarehouseById(id);
            if (warehouse == null)
                return RedirectToAction("Warehouses");

            _shippingService.DeleteWarehouse(warehouse);

            //activity log
            _customerActivityService.InsertActivity("DeleteWarehouse",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteWarehouse"), warehouse.Id), warehouse);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.warehouses.Deleted"));

            return RedirectToAction("Warehouses");
        }

        #endregion

        #region Restrictions

        public virtual IActionResult Restrictions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _shippingModelFactory.PrepareShippingMethodRestrictionModel(new ShippingMethodRestrictionModel());

            return View(model);
        }

        //we ignore this filter for increase RequestFormLimits
        [IgnoreAntiforgeryToken]
        //we use 2048 value because in some cases default value (1024) is too small for this action
        [RequestFormLimits(ValueCountLimit = 2048)]
        [HttpPost, ActionName("Restrictions")]
        public virtual IActionResult RestrictionSave(ShippingMethodRestrictionModel model, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var countries = _countryService.GetAllCountries(showHidden: true);
            var shippingMethods = _shippingService.GetAllShippingMethods();

            foreach (var shippingMethod in shippingMethods)
            {
                var formKey = "restrict_" + shippingMethod.Id;
                var countryIdsToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList()
                    : new List<int>();

                foreach (var country in countries)
                {
                    var restrict = countryIdsToRestrict.Contains(country.Id);
                    var shippingMethodCountryMappings =
                        _shippingService.GetShippingMethodCountryMapping(shippingMethod.Id, country.Id);

                    if (restrict)
                    {
                        if (shippingMethodCountryMappings.Any())
                            continue;

                        _shippingService.InsertShippingMethodCountryMapping(new ShippingMethodCountryMapping { CountryId = country.Id, ShippingMethodId = shippingMethod.Id});
                        _shippingService.UpdateShippingMethod(shippingMethod);
                    }
                    else
                    {
                        if (!shippingMethodCountryMappings.Any())
                            continue;

                        _shippingService.DeleteShippingMethodCountryMapping(shippingMethodCountryMappings.FirstOrDefault());
                        _shippingService.UpdateShippingMethod(shippingMethod);
                    }
                }
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Restrictions.Updated"));

            return RedirectToAction("Restrictions");
        }

        #endregion
    }
}