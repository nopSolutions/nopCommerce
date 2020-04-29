using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Cache;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the implementation of the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial class BaseAdminModelFactory : IBaseAdminModelFactory
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IPluginService _pluginService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public BaseAdminModelFactory(ICacheKeyService cacheKeyService,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPluginService pluginService,
            IProductTemplateService productTemplateService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            ITopicTemplateService topicTemplateService,
            IVendorService vendorService)
        {
            _cacheKeyService = cacheKeyService;
            _categoryService = categoryService;
            _categoryTemplateService = categoryTemplateService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _languageService = languageService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _pluginService = pluginService;
            _productTemplateService = productTemplateService;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _storeService = storeService;
            _taxCategoryService = taxCategoryService;
            _topicTemplateService = topicTemplateService;
            _vendorService = vendorService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare default item
        /// </summary>
        /// <param name="items">Available items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
        protected virtual void PrepareDefaultItem(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //at now we use "0" as the default value
            const string value = "0";

            //prepare item text
            defaultItemText ??= _localizationService.GetResource("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = value });
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        protected virtual List<SelectListItem> GetCategoryList(bool showHidden = true)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoriesListKey, showHidden);
            var listItems = _staticCacheManager.Get(cacheKey, () =>
            {
                var categories = _categoryService.GetAllCategories(showHidden: showHidden);
                return categories.Select(c => new SelectListItem
                {
                    Text = _categoryService.GetFormattedBreadCrumb(c, categories),
                    Value = c.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get manufacturer list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer list</returns>
        protected virtual List<SelectListItem> GetManufacturerList(bool showHidden = true)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturersListKey, showHidden);
            var listItems = _staticCacheManager.Get(cacheKey, () =>
            {
                var manufacturers = _manufacturerService.GetAllManufacturers(showHidden: showHidden);
                return manufacturers.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get vendor list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendor list</returns>
        protected virtual List<SelectListItem> GetVendorList(bool showHidden = true)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.VendorsListKey, showHidden);
            var listItems = _staticCacheManager.Get(cacheKey, () =>
            {
                var vendors = _vendorService.GetAllVendors(showHidden: showHidden);
                return vendors.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare available activity log types
        /// </summary>
        /// <param name="items">Activity log type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareActivityLogTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available activity log types
            var availableActivityTypes = _customerActivityService.GetAllActivityTypes();
            foreach (var activityType in availableActivityTypes)
            {
                items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available order statuses
        /// </summary>
        /// <param name="items">Order status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareOrderStatuses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            var availableStatusItems = OrderStatus.Pending.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available payment statuses
        /// </summary>
        /// <param name="items">Payment status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PreparePaymentStatuses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available payment statuses
            var availableStatusItems = PaymentStatus.Pending.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available shipping statuses
        /// </summary>
        /// <param name="items">Shipping status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareShippingStatuses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available shipping statuses
            var availableStatusItems = ShippingStatus.NotYetShipped.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available countries
        /// </summary>
        /// <param name="items">Country items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCountries(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available countries
            var availableCountries = _countryService.GetAllCountries(showHidden: true);
            foreach (var country in availableCountries)
            {
                items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText ?? _localizationService.GetResource("Admin.Address.SelectCountry"));
        }

        /// <summary>
        /// Prepare available states and provinces
        /// </summary>
        /// <param name="items">State and province items</param>
        /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareStatesAndProvinces(IList<SelectListItem> items, int? countryId,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (countryId.HasValue)
            {
                //prepare available states and provinces of the country
                var availableStates = _stateProvinceService.GetStateProvincesByCountryId(countryId.Value, showHidden: true);
                foreach (var state in availableStates)
                {
                    items.Add(new SelectListItem { Value = state.Id.ToString(), Text = state.Name });
                }

                //insert special item for the default value
                if (items.Any())
                    PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText ?? _localizationService.GetResource("Admin.Address.SelectState"));
            }

            //insert special item for the default value
            if (!items.Any())
                PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText ?? _localizationService.GetResource("Admin.Address.Other"));
        }

        /// <summary>
        /// Prepare available languages
        /// </summary>
        /// <param name="items">Language items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareLanguages(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available languages
            var availableLanguages = _languageService.GetAllLanguages(showHidden: true);
            foreach (var language in availableLanguages)
            {
                items.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available stores
        /// </summary>
        /// <param name="items">Store items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareStores(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            foreach (var store in availableStores)
            {
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available customer roles
        /// </summary>
        /// <param name="items">Customer role items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCustomerRoles(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available customer roles
            var availableCustomerRoles = _customerService.GetAllCustomerRoles();
            foreach (var customerRole in availableCustomerRoles)
            {
                items.Add(new SelectListItem { Value = customerRole.Id.ToString(), Text = customerRole.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available email accounts
        /// </summary>
        /// <param name="items">Email account items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareEmailAccounts(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available email accounts
            var availableEmailAccounts = _emailAccountService.GetAllEmailAccounts();
            foreach (var emailAccount in availableEmailAccounts)
            {
                items.Add(new SelectListItem { Value = emailAccount.Id.ToString(), Text = $"{emailAccount.DisplayName} ({emailAccount.Email})" });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available tax categories
        /// </summary>
        /// <param name="items">Tax category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareTaxCategories(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tax categories
            var availableTaxCategories = _taxCategoryService.GetAllTaxCategories();
            foreach (var taxCategory in availableTaxCategories)
            {
                items.Add(new SelectListItem { Value = taxCategory.Id.ToString(), Text = taxCategory.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem,
                defaultItemText ?? _localizationService.GetResource("Admin.Configuration.Settings.Tax.TaxCategories.None"));
        }

        /// <summary>
        /// Prepare available categories
        /// </summary>
        /// <param name="items">Category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCategories(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available categories
            var availableCategoryItems = GetCategoryList();
            foreach (var categoryItem in availableCategoryItems)
            {
                items.Add(categoryItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available manufacturers
        /// </summary>
        /// <param name="items">Manufacturer items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareManufacturers(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available manufacturers
            var availableManufacturerItems = GetManufacturerList();
            foreach (var manufacturerItem in availableManufacturerItems)
            {
                items.Add(manufacturerItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available vendors
        /// </summary>
        /// <param name="items">Vendor items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareVendors(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available vendors
            var availableVendorItems = GetVendorList();
            foreach (var vendorItem in availableVendorItems)
            {
                items.Add(vendorItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available product types
        /// </summary>
        /// <param name="items">Product type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareProductTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available product types
            var availableProductTypeItems = ProductType.SimpleProduct.ToSelectList(false);
            foreach (var productTypeItem in availableProductTypeItems)
            {
                items.Add(productTypeItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available category templates
        /// </summary>
        /// <param name="items">Category template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCategoryTemplates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available category templates
            var availableTemplates = _categoryTemplateService.GetAllCategoryTemplates();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available time zones
        /// </summary>
        /// <param name="items">Time zone items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareTimeZones(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available time zones
            var availableTimeZones = _dateTimeHelper.GetSystemTimeZones();
            foreach (var timeZone in availableTimeZones)
            {
                items.Add(new SelectListItem { Value = timeZone.Id, Text = timeZone.DisplayName });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available shopping cart types
        /// </summary>
        /// <param name="items">Shopping cart type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareShoppingCartTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available shopping cart types
            var availableShoppingCartTypeItems = ShoppingCartType.ShoppingCart.ToSelectList(false);
            foreach (var shoppingCartTypeItem in availableShoppingCartTypeItems)
            {
                items.Add(shoppingCartTypeItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available tax display types
        /// </summary>
        /// <param name="items">Tax display type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareTaxDisplayTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tax display types
            var availableTaxDisplayTypeItems = TaxDisplayType.ExcludingTax.ToSelectList(false);
            foreach (var taxDisplayTypeItem in availableTaxDisplayTypeItems)
            {
                items.Add(taxDisplayTypeItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available currencies
        /// </summary>
        /// <param name="items">Currency items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCurrencies(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available currencies
            var availableCurrencies = _currencyService.GetAllCurrencies(true);
            foreach (var currency in availableCurrencies)
            {
                items.Add(new SelectListItem { Value = currency.Id.ToString(), Text = currency.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available discount types
        /// </summary>
        /// <param name="items">Discount type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareDiscountTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available discount types
            var availableDiscountTypeItems = DiscountType.AssignedToOrderTotal.ToSelectList(false);
            foreach (var discountTypeItem in availableDiscountTypeItems)
            {
                items.Add(discountTypeItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available log levels
        /// </summary>
        /// <param name="items">Log level items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareLogLevels(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available log levels
            var availableLogLevelItems = LogLevel.Debug.ToSelectList(false);
            foreach (var logLevelItem in availableLogLevelItems)
            {
                items.Add(logLevelItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available manufacturer templates
        /// </summary>
        /// <param name="items">Manufacturer template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareManufacturerTemplates(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available manufacturer templates
            var availableTemplates = _manufacturerTemplateService.GetAllManufacturerTemplates();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available load plugin modes
        /// </summary>
        /// <param name="items">Load plugin mode items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareLoadPluginModes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available load plugin modes
            var availableLoadPluginModeItems = LoadPluginsMode.All.ToSelectList(false);
            foreach (var loadPluginModeItem in availableLoadPluginModeItems)
            {
                items.Add(loadPluginModeItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available plugin groups
        /// </summary>
        /// <param name="items">Plugin group items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PreparePluginGroups(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available plugin groups
            var availablePluginGroups = _pluginService.GetPluginDescriptors<IPlugin>(LoadPluginsMode.All)
                .Select(plugin => plugin.Group).Distinct().OrderBy(groupName => groupName).ToList();
            foreach (var group in availablePluginGroups)
            {
                items.Add(new SelectListItem { Value = group, Text = group });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available return request statuses
        /// </summary>
        /// <param name="items">Return request status items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareReturnRequestStatuses(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available return request statuses
            var availableStatusItems = ReturnRequestStatus.Pending.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available product templates
        /// </summary>
        /// <param name="items">Product template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareProductTemplates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available product templates
            var availableTemplates = _productTemplateService.GetAllProductTemplates();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available topic templates
        /// </summary>
        /// <param name="items">Topic template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareTopicTemplates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available topic templates
            var availableTemplates = _topicTemplateService.GetAllTopicTemplates();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available warehouses
        /// </summary>
        /// <param name="items">Warehouse items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareWarehouses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available warehouses
            var availableWarehouses = _shippingService.GetAllWarehouses();
            foreach (var warehouse in availableWarehouses)
            {
                items.Add(new SelectListItem { Value = warehouse.Id.ToString(), Text = warehouse.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available delivery dates
        /// </summary>
        /// <param name="items">Delivery date items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareDeliveryDates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available delivery dates
            var availableDeliveryDates = _dateRangeService.GetAllDeliveryDates();
            foreach (var date in availableDeliveryDates)
            {
                items.Add(new SelectListItem { Value = date.Id.ToString(), Text = date.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available product availability ranges
        /// </summary>
        /// <param name="items">Product availability range items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareProductAvailabilityRanges(IList<SelectListItem> items,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available product availability ranges
            var availableProductAvailabilityRanges = _dateRangeService.GetAllProductAvailabilityRanges();
            foreach (var range in availableProductAvailabilityRanges)
            {
                items.Add(new SelectListItem { Value = range.Id.ToString(), Text = range.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available GDPR request types
        /// </summary>
        /// <param name="items">Request type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareGdprRequestTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available request types
            var gdprRequestTypeItems = GdprRequestType.ConsentAgree.ToSelectList(false);
            foreach (var gdprRequestTypeItem in gdprRequestTypeItems)
            {
                items.Add(gdprRequestTypeItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }
        #endregion
    }
}