using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Security;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Globalization;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Fields

        protected CookieSettings CookieSettings { get; }
        protected CurrencySettings CurrencySettings { get; }
        protected IAuthenticationService AuthenticationService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerService CustomerService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected ILanguageService LanguageService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IUserAgentHelper UserAgentHelper { get; }
        protected IVendorService VendorService { get; }
        protected IWebHelper WebHelper { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected TaxSettings TaxSettings { get; }

        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private Vendor _cachedVendor;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;
        private TaxDisplayType? _cachedTaxDisplayType;

        #endregion

        #region Ctor

        public WebWorkContext(CookieSettings cookieSettings,
            CurrencySettings currencySettings,
            IAuthenticationService authenticationService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUserAgentHelper userAgentHelper,
            IVendorService vendorService,
            IWebHelper webHelper,
            LocalizationSettings localizationSettings,
            TaxSettings taxSettings)
        {
            CookieSettings = cookieSettings;
            CurrencySettings = currencySettings;
            AuthenticationService = authenticationService;
            CurrencyService = currencyService;
            CustomerService = customerService;
            GenericAttributeService = genericAttributeService;
            HttpContextAccessor = httpContextAccessor;
            LanguageService = languageService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            UserAgentHelper = userAgentHelper;
            VendorService = vendorService;
            WebHelper = webHelper;
            LocalizationSettings = localizationSettings;
            TaxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get nop customer cookie
        /// </summary>
        /// <returns>String value of cookie</returns>
        protected virtual string GetCustomerCookie()
        {
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            return HttpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        }

        /// <summary>
        /// Set nop customer cookie
        /// </summary>
        /// <param name="customerGuid">Guid of the customer</param>
        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (HttpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
                return;

            //delete current cookie value
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            HttpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = CookieSettings.CustomerCookieExpires;
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (customerGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate,
                Secure = WebHelper.IsCurrentConnectionSecured()
            };
            HttpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, customerGuid.ToString(), options);
        }

        /// <summary>
        /// Set language culture cookie
        /// </summary>
        /// <param name="language">Language</param>
        protected virtual void SetLanguageCookie(Language language)
        {
            if (HttpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
                return;

            //delete current cookie value
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CultureCookie}";
            HttpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            if (string.IsNullOrEmpty(language?.LanguageCulture))
                return;

            //set new cookie value
            var value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language.LanguageCulture));
            var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
            HttpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, value, options);
        }

        /// <summary>
        /// Get language from the request
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the found language
        /// </returns>
        protected virtual async Task<Language> GetLanguageFromRequestAsync()
        {
            var requestCultureFeature = HttpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
            if (requestCultureFeature is null)
                return null;

            //whether we should detect the current language by customer settings
            if (requestCultureFeature.Provider is not NopSeoUrlCultureProvider && !LocalizationSettings.AutomaticallyDetectLanguage)
                return null;

            //get request culture
            if (requestCultureFeature.RequestCulture is null)
                return null;

            //try to get language by culture name
            var requestLanguage = (await LanguageService.GetAllLanguagesAsync()).FirstOrDefault(language =>
                language.LanguageCulture.Equals(requestCultureFeature.RequestCulture.Culture.Name, StringComparison.InvariantCultureIgnoreCase));

            //check language availability
            if (requestLanguage == null || !requestLanguage.Published || !await StoreMappingService.AuthorizeAsync(requestLanguage))
                return null;

            return requestLanguage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current customer
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<Customer> GetCurrentCustomerAsync()
        {
            //whether there is a cached value
            if (_cachedCustomer != null)
                return _cachedCustomer;

            await SetCurrentCustomerAsync();

            return _cachedCustomer;
        }

        /// <summary>
        /// Sets the current customer
        /// </summary>
        /// <param name="customer">Current customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetCurrentCustomerAsync(Customer customer = null)
        {
            if (customer == null)
            {
                //check whether request is made by a background (schedule) task
                if (HttpContextAccessor.HttpContext?.Request
                    ?.Path.Equals(new PathString($"/{NopTaskDefaults.ScheduleTaskPath}"), StringComparison.InvariantCultureIgnoreCase)
                    ?? true)
                {
                    //in this case return built-in customer record for background task
                    customer = await CustomerService.GetOrCreateBackgroundTaskUserAsync();
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //check whether request is made by a search engine, in this case return built-in customer record for search engines
                    if (UserAgentHelper.IsSearchEngine())
                        customer = await CustomerService.GetOrCreateSearchEngineUserAsync();
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //try to get registered user
                    customer = await AuthenticationService.GetAuthenticatedCustomerAsync();
                }

                if (customer != null && !customer.Deleted && customer.Active && !customer.RequireReLogin)
                {
                    //get impersonate user if required
                    var impersonatedCustomerId = await GenericAttributeService
                        .GetAttributeAsync<int?>(customer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute);
                    if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                    {
                        var impersonatedCustomer = await CustomerService.GetCustomerByIdAsync(impersonatedCustomerId.Value);
                        if (impersonatedCustomer != null && !impersonatedCustomer.Deleted &&
                            impersonatedCustomer.Active &&
                            !impersonatedCustomer.RequireReLogin)
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
                    if (Guid.TryParse(customerCookie, out var customerGuid))
                    {
                        //get customer from cookie (should not be registered)
                        var customerByCookie = await CustomerService.GetCustomerByGuidAsync(customerGuid);
                        if (customerByCookie != null && !await CustomerService.IsRegisteredAsync(customerByCookie))
                            customer = customerByCookie;
                    }
                }

                if (customer == null || customer.Deleted || !customer.Active || customer.RequireReLogin)
                {
                    //create guest if not exists
                    customer = await CustomerService.InsertGuestCustomerAsync();
                }
            }

            if (!customer.Deleted && customer.Active && !customer.RequireReLogin)
            {
                //set customer cookie
                SetCustomerCookie(customer.CustomerGuid);

                //cache the found customer
                _cachedCustomer = customer;
            }
        }

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        public virtual Customer OriginalCustomerIfImpersonated => _originalCustomerIfImpersonated;

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<Vendor> GetCurrentVendorAsync()
        {
            //whether there is a cached value
            if (_cachedVendor != null)
                return _cachedVendor;

            var customer = await GetCurrentCustomerAsync();
            if (customer == null)
                return null;

            //check vendor availability
            var vendor = await VendorService.GetVendorByIdAsync(customer.VendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return null;

            //cache the found vendor
            _cachedVendor = vendor;

            return _cachedVendor;
        }

        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetWorkingLanguageAsync(Language language)
        {
            //save passed language identifier
            var customer = await GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.LanguageIdAttribute, language?.Id ?? 0, store.Id);

            //set cookie
            SetLanguageCookie(language);

            //then reset the cached value
            _cachedLanguage = null;
        }

        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<Language> GetWorkingLanguageAsync()
        {
            //whether there is a cached value
            if (_cachedLanguage != null)
                return _cachedLanguage;

            var customer = await GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();

            //whether we should detect the language from the request
            var detectedLanguage = await GetLanguageFromRequestAsync();

            //get current saved language identifier
            var currentLanguageId = await GenericAttributeService
                .GetAttributeAsync<int>(customer, NopCustomerDefaults.LanguageIdAttribute, store.Id);

            //if the language is detected we need to save it
            if (detectedLanguage != null)
            {
                //save the detected language identifier if it differs from the current one
                if (detectedLanguage.Id != currentLanguageId)
                    await SetWorkingLanguageAsync(detectedLanguage);
            }
            else
            {
                var allStoreLanguages = await LanguageService.GetAllLanguagesAsync(storeId: store.Id);

                //check customer language availability
                detectedLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == currentLanguageId);

                //it not found, then try to get the default language for the current store (if specified)
                detectedLanguage ??= allStoreLanguages.FirstOrDefault(language => language.Id == store.DefaultLanguageId);

                //if the default language for the current store not found, then try to get the first one
                detectedLanguage ??= allStoreLanguages.FirstOrDefault();

                //if there are no languages for the current store try to get the first one regardless of the store
                detectedLanguage ??= (await LanguageService.GetAllLanguagesAsync()).FirstOrDefault();

                SetLanguageCookie(detectedLanguage);
            }

            //cache the found language
            _cachedLanguage = detectedLanguage;

            return _cachedLanguage;
        }

        /// <summary>
        /// Gets current user working currency
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<Currency> GetWorkingCurrencyAsync()
        {
            //whether there is a cached value
            if (_cachedCurrency != null)
                return _cachedCurrency;

            var adminAreaUrl = $"{WebHelper.GetStoreLocation()}admin";

            //return primary store currency when we're in admin area/mode
            if (WebHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                var primaryStoreCurrency = await CurrencyService.GetCurrencyByIdAsync(CurrencySettings.PrimaryStoreCurrencyId);
                if (primaryStoreCurrency != null)
                {
                    _cachedCurrency = primaryStoreCurrency;
                    return primaryStoreCurrency;
                }
            }

            var customer = await GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();

            //find a currency previously selected by a customer
            var customerCurrencyId = await GenericAttributeService
                .GetAttributeAsync<int>(customer, NopCustomerDefaults.CurrencyIdAttribute, store.Id);

            var allStoreCurrencies = await CurrencyService.GetAllCurrenciesAsync(storeId: store.Id);

            //check customer currency availability
            var customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == customerCurrencyId);
            if (customerCurrency == null)
            {
                //it not found, then try to get the default currency for the current language (if specified)
                var language = await GetWorkingLanguageAsync();
                customerCurrency = allStoreCurrencies
                    .FirstOrDefault(currency => currency.Id == language.DefaultCurrencyId);
            }

            //if the default currency for the current store not found, then try to get the first one
            if (customerCurrency == null)
                customerCurrency = allStoreCurrencies.FirstOrDefault();

            //if there are no currencies for the current store try to get the first one regardless of the store
            if (customerCurrency == null)
                customerCurrency = (await CurrencyService.GetAllCurrenciesAsync()).FirstOrDefault();

            //cache the found currency
            _cachedCurrency = customerCurrency;

            return _cachedCurrency;
        }

        /// <summary>
        /// Sets current user working currency
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetWorkingCurrencyAsync(Currency currency)
        {
            //save passed currency identifier
            var customer = await GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CurrencyIdAttribute, currency?.Id ?? 0, store.Id);

            //then reset the cached value
            _cachedCurrency = null;
        }

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<TaxDisplayType> GetTaxDisplayTypeAsync()
        {
            //whether there is a cached value
            if (_cachedTaxDisplayType.HasValue)
                return _cachedTaxDisplayType.Value;

            var taxDisplayType = TaxDisplayType.IncludingTax;
            var customer = await GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();

            //whether customers are allowed to select tax display type
            if (TaxSettings.AllowCustomersToSelectTaxDisplayType && customer != null)
            {
                //try to get previously saved tax display type
                var taxDisplayTypeId = await GenericAttributeService
                    .GetAttributeAsync<int?>(customer, NopCustomerDefaults.TaxDisplayTypeIdAttribute, store.Id);
                if (taxDisplayTypeId.HasValue)
                    taxDisplayType = (TaxDisplayType)taxDisplayTypeId.Value;
                else
                {
                    //default tax type by customer roles
                    var defaultRoleTaxDisplayType = await CustomerService.GetCustomerDefaultTaxDisplayTypeAsync(customer);
                    if (defaultRoleTaxDisplayType != null)
                        taxDisplayType = defaultRoleTaxDisplayType.Value;
                }
            }
            else
            {
                //default tax type by customer roles
                var defaultRoleTaxDisplayType = await CustomerService.GetCustomerDefaultTaxDisplayTypeAsync(customer);
                if (defaultRoleTaxDisplayType != null)
                    taxDisplayType = defaultRoleTaxDisplayType.Value;
                else
                {
                    //or get the default tax display type
                    taxDisplayType = TaxSettings.TaxDisplayType;
                }
            }

            //cache the value
            _cachedTaxDisplayType = taxDisplayType;

            return _cachedTaxDisplayType.Value;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType)
        {
            //whether customers are allowed to select tax display type
            if (!TaxSettings.AllowCustomersToSelectTaxDisplayType)
                return;

            //save passed value
            var customer = await GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            await GenericAttributeService
                .SaveAttributeAsync(customer, NopCustomerDefaults.TaxDisplayTypeIdAttribute, (int)taxDisplayType, store.Id);

            //then reset the cached value
            _cachedTaxDisplayType = null;
        }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}