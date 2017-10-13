using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ShippingController : BaseAdminController
	{
		#region Fields

        private readonly IShippingService _shippingService;
        private readonly ShippingSettings _shippingSettings;
        private readonly ISettingService _settingService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
	    private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor

        public ShippingController(IShippingService shippingService, 
            ShippingSettings shippingSettings,
            ISettingService settingService,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ILocalizationService localizationService, 
            IPermissionService permissionService,
             ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            IDateRangeService dateRangeService,
            IPluginFinder pluginFinder,
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService)
        {
            this._shippingService = shippingService;
            this._shippingSettings = shippingSettings;
            this._settingService = settingService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._dateRangeService = dateRangeService;
            this._pluginFinder = pluginFinder;
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;
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

            return View();
        }

        [HttpPost]
        public virtual IActionResult Providers(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var shippingProvidersModel = new List<ShippingRateComputationMethodModel>();
            var shippingProviders = _shippingService.LoadAllShippingRateComputationMethods();
            foreach (var shippingProvider in shippingProviders)
            {
                var tmp1 = shippingProvider.ToModel();
                tmp1.IsActive = shippingProvider.IsShippingRateComputationMethodActive(_shippingSettings);
                tmp1.LogoUrl = shippingProvider.PluginDescriptor.GetLogoUrl(_webHelper);
                tmp1.ConfigurationUrl = shippingProvider.GetConfigurationPageUrl();
                shippingProvidersModel.Add(tmp1);
            }
            shippingProvidersModel = shippingProvidersModel.ToList();
            var gridModel = new DataSourceResult
            {
                Data = shippingProvidersModel,
                Total = shippingProvidersModel.Count()
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ProviderUpdate(ShippingRateComputationMethodModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(model.SystemName);
            if (srcm.IsShippingRateComputationMethodActive(_shippingSettings))
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
            PluginManager.SavePluginDescriptor(pluginDescriptor);

            //reset plugin cache
            _pluginFinder.ReloadPlugins();

            return new NullJsonResult();
        }

        #endregion
        
        #region Pickup point providers

        public virtual IActionResult PickupPointProviders()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult PickupPointProviders(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var pickupPointProviderModel = new List<PickupPointProviderModel>();
            var allProviders = _shippingService.LoadAllPickupPointProviders();
            foreach (var provider in allProviders)
            {
                var model = provider.ToModel();
                model.IsActive = provider.IsPickupPointProviderActive(_shippingSettings);
                model.LogoUrl = provider.PluginDescriptor.GetLogoUrl(_webHelper);
                model.ConfigurationUrl = provider.GetConfigurationPageUrl();
                pickupPointProviderModel.Add(model);
            }

            var gridModel = new DataSourceResult
            {
                Data = pickupPointProviderModel,
                Total = pickupPointProviderModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult PickupPointProviderUpdate(PickupPointProviderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var pickupPointProvider = _shippingService.LoadPickupPointProviderBySystemName(model.SystemName);
            if (pickupPointProvider.IsPickupPointProviderActive(_shippingSettings))
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
            PluginManager.SavePluginDescriptor(pluginDescriptor);

            //reset plugin cache
            _pluginFinder.ReloadPlugins();

            return new NullJsonResult();
        }

        #endregion
        
        #region Shipping methods

        public virtual IActionResult Methods()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult Methods(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult CreateMethod()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ShippingMethodModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateMethod(ShippingMethodModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var sm = model.ToEntity();
                _shippingService.InsertShippingMethod(sm);
                //locales
                UpdateLocales(sm, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Methods.Added"));
                return continueEditing ? RedirectToAction("EditMethod", new { id = sm.Id }) : RedirectToAction("Methods");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditMethod(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sm = _shippingService.GetShippingMethodById(id);
            if (sm == null)
                //No shipping method found with the specified id
                return RedirectToAction("Methods");

            var model = sm.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = sm.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = sm.GetLocalized(x => x.Description, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditMethod(ShippingMethodModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sm = _shippingService.GetShippingMethodById(model.Id);
            if (sm == null)
                //No shipping method found with the specified id
                return RedirectToAction("Methods");

            if (ModelState.IsValid)
            {
                sm = model.ToEntity(sm);
                _shippingService.UpdateShippingMethod(sm);
                //locales
                UpdateLocales(sm, model);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Methods.Updated"));
                return continueEditing ? RedirectToAction("EditMethod", sm.Id) : RedirectToAction("Methods");
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteMethod(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var sm = _shippingService.GetShippingMethodById(id);
            if (sm == null)
                //No shipping method found with the specified id
                return RedirectToAction("Methods");

            _shippingService.DeleteShippingMethod(sm);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Methods.Deleted"));
            return RedirectToAction("Methods");
        }
        
        #endregion
        
        #region Dates and ranges

        public virtual IActionResult DatesAndRanges()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            return View();
        }

	    #endregion

        #region Delivery dates

        [HttpPost]
        public virtual IActionResult DeliveryDates(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var deliveryDatesModel = _dateRangeService.GetAllDeliveryDates().Select(x => x.ToModel()).ToList();
            var gridModel = new DataSourceResult
            {
                Data = deliveryDatesModel,
                Total = deliveryDatesModel.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult CreateDeliveryDate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new DeliveryDateModel();
            
            //locales
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateDeliveryDate(DeliveryDateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var deliveryDate = model.ToEntity();
                _dateRangeService.InsertDeliveryDate(deliveryDate);
                
                //locales
                UpdateLocales(deliveryDate, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.DeliveryDates.Added"));

                return continueEditing ? RedirectToAction("EditDeliveryDate", new { id = deliveryDate.Id }) : RedirectToAction("DatesAndRanges");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditDeliveryDate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var deliveryDate = _dateRangeService.GetDeliveryDateById(id);
            if (deliveryDate == null)
                //No delivery date found with the specified id
                return RedirectToAction("DatesAndRanges");

            var model = deliveryDate.ToModel();
            
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = deliveryDate.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditDeliveryDate(DeliveryDateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var deliveryDate = _dateRangeService.GetDeliveryDateById(model.Id);
            if (deliveryDate == null)
                //No delivery date found with the specified id
                return RedirectToAction("DatesAndRanges");

            if (ModelState.IsValid)
            {
                deliveryDate = model.ToEntity(deliveryDate);
                _dateRangeService.UpdateDeliveryDate(deliveryDate);
                
                //locales
                UpdateLocales(deliveryDate, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.DeliveryDates.Updated"));

                return continueEditing ? RedirectToAction("EditDeliveryDate", deliveryDate.Id) : RedirectToAction("DatesAndRanges");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteDeliveryDate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var deliveryDate = _dateRangeService.GetDeliveryDateById(id);
            if (deliveryDate == null)
                //No delivery date found with the specified id
                return RedirectToAction("DatesAndRanges");

            _dateRangeService.DeleteDeliveryDate(deliveryDate);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.DeliveryDates.Deleted"));

            return RedirectToAction("DatesAndRanges");
        }

        #endregion
        
        #region Product availability ranges

        [HttpPost]
        public virtual IActionResult ProductAvailabilityRanges(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var productAvailabilityRangesModel = _dateRangeService.GetAllProductAvailabilityRanges().Select(range => range.ToModel()).ToList();
            var gridModel = new DataSourceResult
            {
                Data = productAvailabilityRangesModel,
                Total = productAvailabilityRangesModel.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult CreateProductAvailabilityRange()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ProductAvailabilityRangeModel();
            
            //locales
            AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAvailabilityRange = model.ToEntity();
                _dateRangeService.InsertProductAvailabilityRange(productAvailabilityRange);
                
                //locales
                UpdateLocales(productAvailabilityRange, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.ProductAvailabilityRanges.Added"));

                return continueEditing ? RedirectToAction("EditProductAvailabilityRange", new { id = productAvailabilityRange.Id }) : RedirectToAction("DatesAndRanges");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditProductAvailabilityRange(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(id);
            if (productAvailabilityRange == null)
                //No availability range found with the specified id
                return RedirectToAction("DatesAndRanges");

            var model = productAvailabilityRange.ToModel();
            
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productAvailabilityRange.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditProductAvailabilityRange(ProductAvailabilityRangeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(model.Id);
            if (productAvailabilityRange == null)
                //No availability range found with the specified id
                return RedirectToAction("DatesAndRanges");

            if (ModelState.IsValid)
            {
                productAvailabilityRange = model.ToEntity(productAvailabilityRange);
                _dateRangeService.UpdateProductAvailabilityRange(productAvailabilityRange);
                
                //locales
                UpdateLocales(productAvailabilityRange, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.ProductAvailabilityRanges.Updated"));

                return continueEditing ? RedirectToAction("EditProductAvailabilityRange", productAvailabilityRange.Id) : RedirectToAction("DatesAndRanges");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteProductAvailabilityRange(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(id);
            if (productAvailabilityRange == null)
                //No availability range found with the specified id
                return RedirectToAction("DatesAndRanges");

            _dateRangeService.DeleteProductAvailabilityRange(productAvailabilityRange);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.ProductAvailabilityRanges.Deleted"));

            return RedirectToAction("DatesAndRanges");
        }

        #endregion
        
        #region Warehouses

        public virtual IActionResult Warehouses()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult Warehouses(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedKendoGridJson();

            var warehousesModel = _shippingService.GetAllWarehouses()
                .Select(x =>
                {
                    var warehouseModel = new WarehouseModel
                    {
                        Id = x.Id,
                        Name = x.Name
                        //ignore address for list view (performance optimization)
                    };
                    return warehouseModel;
                })
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = warehousesModel,
                Total = warehousesModel.Count
            };

            return Json(gridModel);
        }


        public virtual IActionResult CreateWarehouse()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new WarehouseModel();
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            model.Address.CountryEnabled = true;
            model.Address.CountryRequired = true;
            model.Address.StateProvinceEnabled = true;
            model.Address.CityEnabled = true;
            model.Address.StreetAddressEnabled = true;
            model.Address.ZipPostalCodeEnabled = true;
            model.Address.ZipPostalCodeRequired = true;
            model.Address.PhoneEnabled = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateWarehouse(WarehouseModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                _addressService.InsertAddress(address);
                var warehouse = new Warehouse
                {
                    Name = model.Name,
                    AdminComment = model.AdminComment,
                    AddressId = address.Id
                };

                _shippingService.InsertWarehouse(warehouse);
                
                //activity log
                _customerActivityService.InsertActivity("AddNewWarehouse", _localizationService.GetResource("ActivityLog.AddNewWarehouse"), warehouse.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Added"));
                return continueEditing ? RedirectToAction("EditWarehouse", new { id = warehouse.Id }) : RedirectToAction("Warehouses");
            }

            //If we got this far, something failed, redisplay form
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        public virtual IActionResult EditWarehouse(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var warehouse = _shippingService.GetWarehouseById(id);
            if (warehouse == null)
                //No warehouse found with the specified id
                return RedirectToAction("Warehouses");

            var address = _addressService.GetAddressById(warehouse.AddressId);
            var model = new WarehouseModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                AdminComment = warehouse.AdminComment
            };
            
            if (address != null)
            {
                model.Address = address.ToModel();
            }
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (address != null && c.Id == address.CountryId) });
            //states
            var states = address != null && address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            model.Address.CountryEnabled = true;
            model.Address.CountryRequired = true;
            model.Address.StateProvinceEnabled = true;
            model.Address.CityEnabled = true;
            model.Address.StreetAddressEnabled = true;
            model.Address.ZipPostalCodeEnabled = true;
            model.Address.ZipPostalCodeRequired = true;
            model.Address.PhoneEnabled = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditWarehouse(WarehouseModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var warehouse = _shippingService.GetWarehouseById(model.Id);
            if (warehouse == null)
                //No warehouse found with the specified id
                return RedirectToAction("Warehouses");

            if (ModelState.IsValid)
            {
                var address = _addressService.GetAddressById(warehouse.AddressId) ??
                    new Core.Domain.Common.Address
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                address = model.Address.ToEntity(address);
                if (address.Id > 0)
                    _addressService.UpdateAddress(address);
                else
                    _addressService.InsertAddress(address);

                warehouse.Name = model.Name;
                warehouse.AdminComment = model.AdminComment;
                warehouse.AddressId = address.Id;

                _shippingService.UpdateWarehouse(warehouse);

                //activity log
                _customerActivityService.InsertActivity("EditWarehouse", _localizationService.GetResource("ActivityLog.EditWarehouse"), warehouse.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Updated"));
                return continueEditing ? RedirectToAction("EditWarehouse", warehouse.Id) : RedirectToAction("Warehouses");
            }

            //If we got this far, something failed, redisplay form

            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteWarehouse(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var warehouse = _shippingService.GetWarehouseById(id);
            if (warehouse == null)
                //No warehouse found with the specified id
                return RedirectToAction("Warehouses");

            _shippingService.DeleteWarehouse(warehouse);

            //activity log
            _customerActivityService.InsertActivity("DeleteWarehouse", _localizationService.GetResource("ActivityLog.DeleteWarehouse"), warehouse.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.warehouses.Deleted"));
            return RedirectToAction("Warehouses");
        }

        #endregion
        
        #region Restrictions

        public virtual IActionResult Restrictions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ShippingMethodRestrictionModel();

            var countries = _countryService.GetAllCountries(showHidden: true);
            var shippingMethods = _shippingService.GetAllShippingMethods();
            foreach (var country in countries)
            {
                model.AvailableCountries.Add(new CountryModel
                    {
                        Id = country.Id,
                        Name = country.Name
                    });
            }
            foreach (var sm in shippingMethods)
            {
                model.AvailableShippingMethods.Add(new ShippingMethodModel
                {
                    Id = sm.Id,
                    Name = sm.Name
                });
            }
            foreach (var country in countries)
                foreach (var shippingMethod in shippingMethods)
                {
                    var restricted = shippingMethod.CountryRestrictionExists(country.Id);
                    if (!model.Restricted.ContainsKey(country.Id))
                        model.Restricted[country.Id] = new Dictionary<int, bool>();
                    model.Restricted[country.Id][shippingMethod.Id] = restricted;
                }

            return View(model);
        }

        [HttpPost, ActionName("Restrictions")]
        public virtual IActionResult RestrictionSave(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var countries = _countryService.GetAllCountries(showHidden: true);
            var shippingMethods = _shippingService.GetAllShippingMethods();


            foreach (var shippingMethod in shippingMethods)
            {
                var formKey = "restrict_" + shippingMethod.Id;
                var countryIdsToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList() 
                    : new List<int>();

                foreach (var country in countries)
                {

                    var restrict = countryIdsToRestrict.Contains(country.Id);
                    if (restrict)
                    {
                        if (shippingMethod.RestrictedCountries.FirstOrDefault(c => c.Id == country.Id) == null)
                        {
                            shippingMethod.RestrictedCountries.Add(country);
                            _shippingService.UpdateShippingMethod(shippingMethod);
                        }
                    }
                    else
                    {
                        if (shippingMethod.RestrictedCountries.FirstOrDefault(c => c.Id == country.Id) != null)
                        {
                            shippingMethod.RestrictedCountries.Remove(country);
                            _shippingService.UpdateShippingMethod(shippingMethod);
                        }
                    }
                }
            }

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Shipping.Restrictions.Updated"));
            return RedirectToAction("Restrictions");
        }
        
        #endregion
    }
}