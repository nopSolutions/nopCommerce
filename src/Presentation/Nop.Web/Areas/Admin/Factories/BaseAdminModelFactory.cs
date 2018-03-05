using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
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
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
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
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
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
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
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
        /// <param name="text">Item text; pass null to use "All" text</param>
        protected virtual void PrepareDefaultItem(IList<SelectListItem> items, string text = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //at now we use "0" as the default value
            var value = "0";

            //prepare item text
            text = text ?? _localizationService.GetResource("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = text, Value = value });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare available activity log types
        /// </summary>
        /// <param name="items">Activity log type items</param>
        public virtual void PrepareActivityLogTypes(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available activity log types
            var availableActivityTypes = _customerActivityService.GetAllActivityTypes();
            foreach (var activityType in availableActivityTypes)
            {
                items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
            }

            //insert special type item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available order statuses
        /// </summary>
        /// <param name="items">Order status items</param>
        public virtual void PrepareOrderStatuses(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available order statuses
            var availableStatusItems = OrderStatus.Pending.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special status item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available payment statuses
        /// </summary>
        /// <param name="items">Payment status items</param>
        public virtual void PreparePaymentStatuses(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available payment statuses
            var availableStatusItems = PaymentStatus.Pending.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special status item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available shipping statuses
        /// </summary>
        /// <param name="items">Shipping status items</param>
        public virtual void PrepareShippingStatuses(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available shipping statuses
            var availableStatusItems = ShippingStatus.NotYetShipped.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }

            //insert special status item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available countries
        /// </summary>
        /// <param name="items">Country items</param>
        public virtual void PrepareCountries(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available countries
            var availableCountries = _countryService.GetAllCountries(showHidden: true);
            foreach (var country in availableCountries)
            {
                items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
            }

            //insert special country item for the "select" value
            PrepareDefaultItem(items, _localizationService.GetResource("Admin.Address.SelectCountry"));
        }

        /// <summary>
        /// Prepare available states and provinces
        /// </summary>
        /// <param name="items">State and province items</param>
        /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
        public virtual void PrepareStatesAndProvinces(IList<SelectListItem> items, int? countryId)
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
            }

            //insert special state item for the "non US" value
            if (!items.Any())
                PrepareDefaultItem(items, _localizationService.GetResource("Admin.Address.OtherNonUS"));
        }

        /// <summary>
        /// Prepare available languages
        /// </summary>
        /// <param name="items">Language items</param>
        public virtual void PrepareLanguages(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available languages
            var availableLanguages = _languageService.GetAllLanguages(showHidden: true);
            foreach (var language in availableLanguages)
            {
                items.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });
            }
        }

        /// <summary>
        /// Prepare available stores
        /// </summary>
        /// <param name="items">Store items</param>
        public virtual void PrepareStores(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available stores
            var availableStores = _storeService.GetAllStores();
            foreach (var store in availableStores)
            {
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
            }

            //insert special store item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available customer roles
        /// </summary>
        /// <param name="items">Customer role items</param>
        public virtual void PrepareCustomerRoles(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available customer roles
            var availableCustomerRoles = _customerService.GetAllCustomerRoles();
            foreach (var customerRole in availableCustomerRoles)
            {
                items.Add(new SelectListItem { Value = customerRole.Id.ToString(), Text = customerRole.Name });
            }

            //insert special customer role item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available email accounts
        /// </summary>
        /// <param name="items">Email account items</param>
        public virtual void PrepareEmailAccounts(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available email accounts
            var availableEmailAccounts = _emailAccountService.GetAllEmailAccounts();
            foreach (var emailAccount in availableEmailAccounts)
            {
                items.Add(new SelectListItem { Value = emailAccount.Id.ToString(), Text = $"{emailAccount.DisplayName} ({emailAccount.Email})" });
            }
        }

        /// <summary>
        /// Prepare available tax categories
        /// </summary>
        /// <param name="items">Tax category items</param>
        public virtual void PrepareTaxCategories(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available tax categories
            var availableTaxCategories = _taxCategoryService.GetAllTaxCategories();
            foreach (var taxCategory in availableTaxCategories)
            {
                items.Add(new SelectListItem { Value = taxCategory.Id.ToString(), Text = taxCategory.Name });
            }

            //insert special tax category item for the "none" value
            PrepareDefaultItem(items, _localizationService.GetResource("Admin.Configuration.Settings.Tax.TaxCategories.None"));
        }

        /// <summary>
        /// Prepare available categories
        /// </summary>
        /// <param name="items">Category items</param>
        public virtual void PrepareCategories(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available categories
            var availableCategoryItems = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var categoryItem in availableCategoryItems)
            {
                items.Add(categoryItem);
            }

            //insert special category item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available manufacturers
        /// </summary>
        /// <param name="items">Manufacturer items</param>
        public virtual void PrepareManufacturers(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available manufacturers
            var availableManufacturerItems = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var manufacturerItem in availableManufacturerItems)
            {
                items.Add(manufacturerItem);
            }

            //insert special manufacturer item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available vendors
        /// </summary>
        /// <param name="items">Vendor items</param>
        public virtual void PrepareVendors(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available vendors
            var availableVendorItems = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var vendorItem in availableVendorItems)
            {
                items.Add(vendorItem);
            }

            //insert special vendor item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available product types
        /// </summary>
        /// <param name="items">Product type items</param>
        public virtual void PrepareProductTypes(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available product types
            var availableProductTypeItems = ProductType.SimpleProduct.ToSelectList(false);
            foreach (var productTypeItem in availableProductTypeItems)
            {
                items.Add(productTypeItem);
            }

            //insert special product type item for the "all" value
            PrepareDefaultItem(items);
        }

        /// <summary>
        /// Prepare available parent categories
        /// </summary>
        /// <param name="items">Category items</param>
        public virtual void PrepareParentCategories(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available parent categories
            var availableCategoryItems = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var categoryItem in availableCategoryItems)
            {
                items.Add(categoryItem);
            }

            //insert special category item for the "none" value
            PrepareDefaultItem(items, _localizationService.GetResource("Admin.Catalog.Categories.Fields.Parent.None"));
        }

        /// <summary>
        /// Prepare available category templates
        /// </summary>
        /// <param name="items">Category template items</param>
        public virtual void PrepareCategoryTemplates(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available category templates
            var availableTemplates = _categoryTemplateService.GetAllCategoryTemplates();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }
        }

        #endregion
    }
}