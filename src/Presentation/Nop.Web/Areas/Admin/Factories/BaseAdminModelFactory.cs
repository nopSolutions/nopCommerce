using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Helpers;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the implementation of the base model factory that implements a most common admin model factories methods
    /// </summary>
    public partial class BaseAdminModelFactory : IBaseAdminModelFactory
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public BaseAdminModelFactory(ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager cacheManager,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            IVendorService vendorService)
        {
            this._categoryService = categoryService;
            this._categoryTemplateService = categoryTemplateService;
            this._countryService = countryService;
            this._currencyService = currencyService;
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._emailAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._stateProvinceService = stateProvinceService;
            this._cacheManager = cacheManager;
            this._storeService = storeService;
            this._taxCategoryService = taxCategoryService;
            this._vendorService = vendorService;
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
            var value = "0";

            //prepare item text
            defaultItemText = defaultItemText ?? _localizationService.GetResource("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = value });
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
                PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText ?? _localizationService.GetResource("Admin.Address.OtherNonUS"));
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
            var availableCategoryItems = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
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
            var availableManufacturerItems = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
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
            var availableVendorItems = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
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

        #endregion
    }
}