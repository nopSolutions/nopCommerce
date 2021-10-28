using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ShippingController : BaseAdminController
    {
        #region Fields

        protected IAddressService AddressService { get; }
        protected ICountryService CountryService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected IDateRangeService DateRangeService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPickupPluginManager PickupPluginManager { get; }
        protected ISettingService SettingService { get; }
        protected IShippingModelFactory ShippingModelFactory { get; }
        protected IShippingPluginManager ShippingPluginManager { get; }
        protected IShippingService ShippingService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }
        protected ShippingSettings ShippingSettings { get; }

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
            AddressService = addressService;
            CountryService = countryService;
            CustomerActivityService = customerActivityService;
            DateRangeService = dateRangeService;
            EventPublisher = eventPublisher;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            PickupPluginManager = pickupPluginManager;
            SettingService = settingService;
            ShippingModelFactory = shippingModelFactory;
            ShippingPluginManager = shippingPluginManager;
            ShippingService = shippingService;
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;
            ShippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(ShippingMethod shippingMethod, ShippingMethodModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(shippingMethod, x => x.Name, localized.Name, localized.LanguageId);
                await LocalizedEntityService.SaveLocalizedValueAsync(shippingMethod, x => x.Description, localized.Description, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(DeliveryDate deliveryDate, DeliveryDateModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(deliveryDate, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(ProductAvailabilityRange productAvailabilityRange, ProductAvailabilityRangeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(productAvailabilityRange, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        #endregion

        #region Shipping rate computation methods

        public virtual async Task<IActionResult> Providers(bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareShippingProviderSearchModelAsync(new ShippingProviderSearchModel());

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Providers(ShippingProviderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ShippingModelFactory.PrepareShippingProviderListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProviderUpdate(ShippingProviderModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var srcm = await ShippingPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
            if (ShippingPluginManager.IsPluginActive(srcm))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    ShippingSettings.ActiveShippingRateComputationMethodSystemNames.Remove(srcm.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(ShippingSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    ShippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(srcm.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(ShippingSettings);
                }
            }

            var pluginDescriptor = srcm.PluginDescriptor;

            //display order
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            await EventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        #endregion

        #region Pickup point providers

        public virtual async Task<IActionResult> PickupPointProviders()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PreparePickupPointProviderSearchModelAsync(new PickupPointProviderSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PickupPointProviders(PickupPointProviderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ShippingModelFactory.PreparePickupPointProviderListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PickupPointProviderUpdate(PickupPointProviderModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPointProvider = await PickupPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
            if (PickupPluginManager.IsPluginActive(pickupPointProvider))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    ShippingSettings.ActivePickupPointProviderSystemNames.Remove(pickupPointProvider.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(ShippingSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    ShippingSettings.ActivePickupPointProviderSystemNames.Add(pickupPointProvider.PluginDescriptor.SystemName);
                    await SettingService.SaveSettingAsync(ShippingSettings);
                }
            }

            var pluginDescriptor = pickupPointProvider.PluginDescriptor;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            await EventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        #endregion

        #region Shipping methods

        public virtual async Task<IActionResult> Methods()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareShippingMethodSearchModelAsync(new ShippingMethodSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Methods(ShippingMethodSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ShippingModelFactory.PrepareShippingMethodListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateMethod()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareShippingMethodModelAsync(new ShippingMethodModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateMethod(ShippingMethodModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var sm = model.ToEntity<ShippingMethod>();
                await ShippingService.InsertShippingMethodAsync(sm);

                //locales
                await UpdateLocalesAsync(sm, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Added"));
                return continueEditing ? RedirectToAction("EditMethod", new { id = sm.Id }) : RedirectToAction("Methods");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareShippingMethodModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditMethod(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a shipping method with the specified id
            var shippingMethod = await ShippingService.GetShippingMethodByIdAsync(id);
            if (shippingMethod == null)
                return RedirectToAction("Methods");

            //prepare model
            var model = await ShippingModelFactory.PrepareShippingMethodModelAsync(null, shippingMethod);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditMethod(ShippingMethodModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a shipping method with the specified id
            var shippingMethod = await ShippingService.GetShippingMethodByIdAsync(model.Id);
            if (shippingMethod == null)
                return RedirectToAction("Methods");

            if (ModelState.IsValid)
            {
                shippingMethod = model.ToEntity(shippingMethod);
                await ShippingService.UpdateShippingMethodAsync(shippingMethod);

                //locales
                await UpdateLocalesAsync(shippingMethod, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Updated"));

                return continueEditing ? RedirectToAction("EditMethod", shippingMethod.Id) : RedirectToAction("Methods");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareShippingMethodModelAsync(model, shippingMethod, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteMethod(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a shipping method with the specified id
            var shippingMethod = await ShippingService.GetShippingMethodByIdAsync(id);
            if (shippingMethod == null)
                return RedirectToAction("Methods");

            await ShippingService.DeleteShippingMethodAsync(shippingMethod);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Methods.Deleted"));

            return RedirectToAction("Methods");
        }

        #endregion

        #region Dates and ranges

        public virtual async Task<IActionResult> DatesAndRanges()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareDatesRangesSearchModelAsync(new DatesRangesSearchModel());

            return View(model);
        }

        #endregion

        #region Delivery dates

        [HttpPost]
        public virtual async Task<IActionResult> DeliveryDates(DeliveryDateSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ShippingModelFactory.PrepareDeliveryDateListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateDeliveryDate()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareDeliveryDateModelAsync(new DeliveryDateModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateDeliveryDate(DeliveryDateModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var deliveryDate = model.ToEntity<DeliveryDate>();
                await DateRangeService.InsertDeliveryDateAsync(deliveryDate);

                //locales
                await UpdateLocalesAsync(deliveryDate, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Added"));

                return continueEditing ? RedirectToAction("EditDeliveryDate", new { id = deliveryDate.Id }) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareDeliveryDateModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditDeliveryDate(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a delivery date with the specified id
            var deliveryDate = await DateRangeService.GetDeliveryDateByIdAsync(id);
            if (deliveryDate == null)
                return RedirectToAction("DatesAndRanges");

            //prepare model
            var model = await ShippingModelFactory.PrepareDeliveryDateModelAsync(null, deliveryDate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditDeliveryDate(DeliveryDateModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a delivery date with the specified id
            var deliveryDate = await DateRangeService.GetDeliveryDateByIdAsync(model.Id);
            if (deliveryDate == null)
                return RedirectToAction("DatesAndRanges");

            if (ModelState.IsValid)
            {
                deliveryDate = model.ToEntity(deliveryDate);
                await DateRangeService.UpdateDeliveryDateAsync(deliveryDate);

                //locales
                await UpdateLocalesAsync(deliveryDate, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Updated"));

                return continueEditing ? RedirectToAction("EditDeliveryDate", deliveryDate.Id) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareDeliveryDateModelAsync(model, deliveryDate, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteDeliveryDate(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a delivery date with the specified id
            var deliveryDate = await DateRangeService.GetDeliveryDateByIdAsync(id);
            if (deliveryDate == null)
                return RedirectToAction("DatesAndRanges");

            await DateRangeService.DeleteDeliveryDateAsync(deliveryDate);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Deleted"));

            return RedirectToAction("DatesAndRanges");
        }

        #endregion

        #region Product availability ranges

        [HttpPost]
        public virtual async Task<IActionResult> ProductAvailabilityRanges(ProductAvailabilityRangeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ShippingModelFactory.PrepareProductAvailabilityRangeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateProductAvailabilityRange()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareProductAvailabilityRangeModelAsync(new ProductAvailabilityRangeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAvailabilityRange = model.ToEntity<ProductAvailabilityRange>();
                await DateRangeService.InsertProductAvailabilityRangeAsync(productAvailabilityRange);

                //locales
                await UpdateLocalesAsync(productAvailabilityRange, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Added"));

                return continueEditing ? RedirectToAction("EditProductAvailabilityRange", new { id = productAvailabilityRange.Id }) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareProductAvailabilityRangeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditProductAvailabilityRange(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a product availability range with the specified id
            var productAvailabilityRange = await DateRangeService.GetProductAvailabilityRangeByIdAsync(id);
            if (productAvailabilityRange == null)
                return RedirectToAction("DatesAndRanges");

            //prepare model
            var model = await ShippingModelFactory.PrepareProductAvailabilityRangeModelAsync(null, productAvailabilityRange);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a product availability range with the specified id
            var productAvailabilityRange = await DateRangeService.GetProductAvailabilityRangeByIdAsync(model.Id);
            if (productAvailabilityRange == null)
                return RedirectToAction("DatesAndRanges");

            if (ModelState.IsValid)
            {
                productAvailabilityRange = model.ToEntity(productAvailabilityRange);
                await DateRangeService.UpdateProductAvailabilityRangeAsync(productAvailabilityRange);

                //locales
                await UpdateLocalesAsync(productAvailabilityRange, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Updated"));

                return continueEditing ? RedirectToAction("EditProductAvailabilityRange", productAvailabilityRange.Id) : RedirectToAction("DatesAndRanges");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareProductAvailabilityRangeModelAsync(model, productAvailabilityRange, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteProductAvailabilityRange(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a product availability range with the specified id
            var productAvailabilityRange = await DateRangeService.GetProductAvailabilityRangeByIdAsync(id);
            if (productAvailabilityRange == null)
                return RedirectToAction("DatesAndRanges");

            await DateRangeService.DeleteProductAvailabilityRangeAsync(productAvailabilityRange);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.ProductAvailabilityRanges.Deleted"));

            return RedirectToAction("DatesAndRanges");
        }

        #endregion

        #region Warehouses

        public virtual async Task<IActionResult> Warehouses()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareWarehouseSearchModelAsync(new WarehouseSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Warehouses(WarehouseSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ShippingModelFactory.PrepareWarehouseListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateWarehouse()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareWarehouseModelAsync(new WarehouseModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateWarehouse(WarehouseModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity<Address>();
                address.CreatedOnUtc = DateTime.UtcNow;
                await AddressService.InsertAddressAsync(address);

                //fill entity from model
                var warehouse = model.ToEntity<Warehouse>();
                warehouse.AddressId = address.Id;

                await ShippingService.InsertWarehouseAsync(warehouse);

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewWarehouse",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewWarehouse"), warehouse.Id), warehouse);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Added"));

                return continueEditing ? RedirectToAction("EditWarehouse", new { id = warehouse.Id }) : RedirectToAction("Warehouses");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareWarehouseModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditWarehouse(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a warehouse with the specified id
            var warehouse = await ShippingService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
                return RedirectToAction("Warehouses");

            //prepare model
            var model = await ShippingModelFactory.PrepareWarehouseModelAsync(null, warehouse);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditWarehouse(WarehouseModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a warehouse with the specified id
            var warehouse = await ShippingService.GetWarehouseByIdAsync(model.Id);
            if (warehouse == null)
                return RedirectToAction("Warehouses");

            if (ModelState.IsValid)
            {
                var address = await AddressService.GetAddressByIdAsync(warehouse.AddressId) ??
                    new Address
                    {
                        CreatedOnUtc = DateTime.UtcNow
                    };
                address = model.Address.ToEntity(address);
                if (address.Id > 0)
                    await AddressService.UpdateAddressAsync(address);
                else
                    await AddressService.InsertAddressAsync(address);

                //fill entity from model
                warehouse = model.ToEntity(warehouse);

                warehouse.AddressId = address.Id;

                await ShippingService.UpdateWarehouseAsync(warehouse);

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditWarehouse",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditWarehouse"), warehouse.Id), warehouse);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Updated"));

                return continueEditing ? RedirectToAction("EditWarehouse", warehouse.Id) : RedirectToAction("Warehouses");
            }

            //prepare model
            model = await ShippingModelFactory.PrepareWarehouseModelAsync(model, warehouse, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteWarehouse(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a warehouse with the specified id
            var warehouse = await ShippingService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
                return RedirectToAction("Warehouses");

            await ShippingService.DeleteWarehouseAsync(warehouse);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteWarehouse",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteWarehouse"), warehouse.Id), warehouse);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.warehouses.Deleted"));

            return RedirectToAction("Warehouses");
        }

        #endregion

        #region Restrictions

        public virtual async Task<IActionResult> Restrictions()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await ShippingModelFactory.PrepareShippingMethodRestrictionModelAsync(new ShippingMethodRestrictionModel());

            return View(model);
        }

        //we ignore this filter for increase RequestFormLimits
        [IgnoreAntiforgeryToken]
        //we use 2048 value because in some cases default value (1024) is too small for this action
        [RequestFormLimits(ValueCountLimit = 2048)]
        [HttpPost, ActionName("Restrictions")]
        public virtual async Task<IActionResult> RestrictionSave(ShippingMethodRestrictionModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var countries = await CountryService.GetAllCountriesAsync(showHidden: true);
            var shippingMethods = await ShippingService.GetAllShippingMethodsAsync();

            var form = model.Form;

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
                        await ShippingService.GetShippingMethodCountryMappingAsync(shippingMethod.Id, country.Id);

                    if (restrict)
                    {
                        if (shippingMethodCountryMappings.Any())
                            continue;

                        await ShippingService.InsertShippingMethodCountryMappingAsync(new ShippingMethodCountryMapping { CountryId = country.Id, ShippingMethodId = shippingMethod.Id});
                        await ShippingService.UpdateShippingMethodAsync(shippingMethod);
                    }
                    else
                    {
                        if (!shippingMethodCountryMappings.Any())
                            continue;

                        await ShippingService.DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMappings.FirstOrDefault());
                        await ShippingService.UpdateShippingMethodAsync(shippingMethod);
                    }
                }
            }

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

            return RedirectToAction("Restrictions");
        }

        #endregion
    }
}