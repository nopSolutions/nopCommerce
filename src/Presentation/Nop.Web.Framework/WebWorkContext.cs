using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Extensions;
using Nop.Core.Http;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Const

        private const string CUSTOMER_COOKIE_NAME = "Nop.customer";

        #endregion

        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrencySettings _currencySettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IVendorService _vendorService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly TaxSettings _taxSettings;
        private readonly IWebHelper _webHelper;

        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private Vendor _cachedVendor;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;
        private TaxDisplayType? _cachedTaxDisplayType;

        #endregion

        #region Ctor

        public WebWorkContext(IHttpContextAccessor httpContextAccessor, 
            CurrencySettings currencySettings,
            IAuthenticationService authenticationService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUserAgentHelper userAgentHelper,
            IVendorService vendorService,
            LocalizationSettings localizationSettings,
            TaxSettings taxSettings,
            IWebHelper webHelper)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._currencySettings = currencySettings;
            this._authenticationService = authenticationService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._languageService = languageService;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._userAgentHelper = userAgentHelper;
            this._vendorService = vendorService;
            this._localizationSettings = localizationSettings;
            this._taxSettings = taxSettings;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get nop customer cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetCustomerCookie()
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Request == null)
                return null;

            return _httpContextAccessor.HttpContext.Request.Cookies[CUSTOMER_COOKIE_NAME];
        }

        /// <summary>
        /// Set nop customer cookie
        /// </summary>
        /// <param name="customerGuid">Guid of the customer</param>
        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Response == null)
                return;

            //delete current cookie value
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CUSTOMER_COOKIE_NAME);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (customerGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CUSTOMER_COOKIE_NAME, customerGuid.ToString(), options);
        }

        /// <summary>
        /// Get language from the requested page URL
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Request == null)
                return null;

            //whether the requsted URL is localized
            var pageUrl = _webHelper.GetRawUrl(_httpContextAccessor.HttpContext.Request);
            var applicationPath = _httpContextAccessor.HttpContext.Request.PathBase.HasValue ?
                _httpContextAccessor.HttpContext.Request.PathBase.Value : "/";
            if (!pageUrl.IsLocalizedUrl(applicationPath, true))
                return null;

            //get language code from the URL
            var seoCode = pageUrl.GetLanguageSeoCodeFromUrl(applicationPath, true);
            if (string.IsNullOrEmpty(seoCode))
                return null;

            //try to get language by language code
            var urlLanguage = _languageService.GetAllLanguages().FirstOrDefault(language =>
                language.UniqueSeoCode.Equals(seoCode, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (urlLanguage == null || !urlLanguage.Published || !_storeMappingService.Authorize(urlLanguage))
                return null;

            return urlLanguage;
        }

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>The found language</returns>
        protected virtual Language GetLanguageFromRequest()
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Request == null)
                return null;

            //get request culture
            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            if (requestCulture == null)
                return null;

            //try to get language by culture name
            var requestLanguage = _languageService.GetAllLanguages().FirstOrDefault(language => 
                language.LanguageCulture.Equals(requestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published || !_storeMappingService.Authorize(requestLanguage))
                return null;

            return requestLanguage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual Customer CurrentCustomer
        {
            get
            {
                //whether there is a cached value
                if (_cachedCustomer != null)
                    return _cachedCustomer;

                Customer customer = null;

                //check whether request is made by a background task
                if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext is FakeHttpContext)
                {
                    //in this case return built-in customer record for background task
                    customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //check whether request is made by a search engine, in this case return built-in customer record for search engines
                    if (_userAgentHelper.IsSearchEngine())
                        customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.SearchEngine);
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //try to get registered user
                    customer = _authenticationService.GetAuthenticatedCustomer();
                }

                if (customer != null && !customer.Deleted && customer.Active && !customer.RequireReLogin)
                {
                    //get impersonate user if required
                    var impersonatedCustomerId = customer.GetAttribute<int?>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
                    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                    {
                        var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active && !impersonatedCustomer.RequireReLogin)
                        {
                            //set impersonated customer
                            _originalCustomerIfImpersonated = customer;
                            customer = impersonatedCustomer;
                        }
                    }
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //get guest customer
                    var customerCookie = GetCustomerCookie();
                    if (!string.IsNullOrEmpty(customerCookie))
                    {
                        if (Guid.TryParse(customerCookie, out Guid customerGuid))
                        {
                            //get customer from cookie (should not be registered)
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null && !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //create guest if not exists
                    customer = _customerService.InsertGuestCustomer();
                }

                if (!customer.Deleted && customer.Active && !customer.RequireReLogin)
                {
                    //set customer cookie
                    SetCustomerCookie(customer.CustomerGuid);

                    //cache the found customer
                    _cachedCustomer = customer;
                }

                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        public virtual Customer OriginalCustomerIfImpersonated
        {
            get { return _originalCustomerIfImpersonated; }
        }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        public virtual Vendor CurrentVendor
        {
            get
            {
                //whether there is a cached value
                if (_cachedVendor != null)
                    return _cachedVendor;

                if (this.CurrentCustomer == null)
                    return null;

                //try to get vendor
                var vendor = _vendorService.GetVendorById(this.CurrentCustomer.VendorId);

                //check vendor availability
                if (vendor == null || vendor.Deleted || !vendor.Active)
                    return null;

                //cache the found vendor
                _cachedVendor = vendor;

                return _cachedVendor;
            }
        }

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                //whether there is a cached value
                if (_cachedLanguage != null)
                    return _cachedLanguage;
                
                Language detectedLanguage = null;

                //localized URLs are enabled, so try to get language from the requested page URL
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                    detectedLanguage = GetLanguageFromUrl();

                //whether we should detect the language from the request
                if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                {
                    //whether language already detected by this way
                    var alreadyDetected = this.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
                        _genericAttributeService, _storeContext.CurrentStore.Id);

                    //if not, try to get language from the request
                    if (!alreadyDetected)
                    {
                        detectedLanguage = GetLanguageFromRequest();
                        if (detectedLanguage != null)
                        {
                            //language already detected
                            _genericAttributeService.SaveAttribute(this.CurrentCustomer, 
                                SystemCustomerAttributeNames.LanguageAutomaticallyDetected, true, _storeContext.CurrentStore.Id);
                        }
                    }
                }

                //if the language is detected we need to save it
                if (detectedLanguage != null)
                {
                    //get current saved language identifier
                    var currentLanguageId = this.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
                        _genericAttributeService, _storeContext.CurrentStore.Id);

                    //save the detected language identifier if it differs from the current one
                    if (detectedLanguage.Id != currentLanguageId)
                    {
                        _genericAttributeService.SaveAttribute(this.CurrentCustomer, 
                            SystemCustomerAttributeNames.LanguageId, detectedLanguage.Id, _storeContext.CurrentStore.Id);
                    }
                }
                
                //get current customer language identifier
                var customerLanguageId = this.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

                var allStoreLanguages = _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id);

                //check customer language availability
                var customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == customerLanguageId);
                if (customerLanguage == null)
                {
                    //it not found, then try to get the default language for the current store (if specified)
                    customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == _storeContext.CurrentStore.DefaultLanguageId);
                }

                //if the default language for the current store not found, then try to get the first one
                if (customerLanguage == null)
                    customerLanguage = allStoreLanguages.FirstOrDefault();

                //if there are no languages for the current store try to get the first one regardless of the store
                if (customerLanguage == null)
                    customerLanguage = _languageService.GetAllLanguages().FirstOrDefault();

                //cache the found language
                _cachedLanguage = customerLanguage;

                return _cachedLanguage;
            }
            set
            {
                //get passed language identifier
                var languageId = value.Return(language => language.Id, 0);

                //and save it
                _genericAttributeService.SaveAttribute(this.CurrentCustomer,
                    SystemCustomerAttributeNames.LanguageId, languageId, _storeContext.CurrentStore.Id);

                //then reset the cache value
                _cachedLanguage = null;
            }
        }

        /// <summary>
        /// Gets or sets current user working currency
        /// </summary>
        public virtual Currency WorkingCurrency
        {
            get
            {
                //whether there is a cached value
                if (_cachedCurrency != null)
                    return _cachedCurrency;
                
                //return primary store currency when we're in admin area/mode
                if (this.IsAdmin)
                {
                    var primaryStoreCurrency =  _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                    if (primaryStoreCurrency != null)
                    {
                        _cachedCurrency = primaryStoreCurrency;
                        return primaryStoreCurrency;
                    }
                }

                //find a currency previously selected by a customer
                var customerCurrencyId = this.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

                var allStoreCurrencies = _currencyService.GetAllCurrencies(storeId: _storeContext.CurrentStore.Id);

                //check customer currency availability
                var customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == customerCurrencyId);
                if (customerCurrency == null)
                {
                    //it not found, then try to get the default currency for the current language (if specified)
                    customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == this.WorkingLanguage.DefaultCurrencyId);
                }

                //if the default currency for the current store not found, then try to get the first one
                if (customerCurrency == null)
                    customerCurrency = allStoreCurrencies.FirstOrDefault();

                //if there are no currencies for the current store try to get the first one regardless of the store
                if (customerCurrency == null)
                    customerCurrency = _currencyService.GetAllCurrencies().FirstOrDefault();

                //cache the found currency
                _cachedCurrency = customerCurrency;

                return _cachedCurrency;
            }
            set
            {
                //get passed currency identifier
                var currencyId = value.Return(currency => currency.Id, 0);

                //and save it
                _genericAttributeService.SaveAttribute(this.CurrentCustomer, 
                    SystemCustomerAttributeNames.CurrencyId, currencyId, _storeContext.CurrentStore.Id);

                //then reset the cache value
                _cachedCurrency = null;
            }
        }

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        public virtual TaxDisplayType TaxDisplayType
        {
            get
            {
                //whether there is a cached value
                if (_cachedTaxDisplayType.HasValue)
                    return _cachedTaxDisplayType.Value;

                TaxDisplayType taxDisplayType;

                //whether customers are allowed to select tax display type
                if (_taxSettings.AllowCustomersToSelectTaxDisplayType && this.CurrentCustomer != null)
                {
                    //try to get previously saved tax display type
                    var taxDisplayTypeId = this.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.TaxDisplayTypeId, 
                        _genericAttributeService, _storeContext.CurrentStore.Id);
                    taxDisplayType = (TaxDisplayType)taxDisplayTypeId;
                }
                else
                {
                    //or get the default tax display type
                    taxDisplayType = _taxSettings.TaxDisplayType;
                }

                //cache the value
                _cachedTaxDisplayType = taxDisplayType;

                return _cachedTaxDisplayType.Value;

            }
            set
            {
                //whether customers are allowed to select tax display type
                if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                    return;

                //save passed value
                _genericAttributeService.SaveAttribute(this.CurrentCustomer, 
                    SystemCustomerAttributeNames.TaxDisplayTypeId, (int)value, _storeContext.CurrentStore.Id);

                //then reset the cache value
                _cachedTaxDisplayType = null;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}
