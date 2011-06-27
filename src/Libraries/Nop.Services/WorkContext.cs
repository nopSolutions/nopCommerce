using System;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Services
{
    /// <summary>
    /// Working context
    /// </summary>
    public partial class WorkContext : IWorkContext
    {
        private const string CustomerCookieName = "Nop.customer";

        private readonly HttpContextBase _httpContext;
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;

        private Customer _cachedCustomer;
        private Customer _originalCustomerIfImpersonated;
        private bool _cachedIsAdmin;

        public WorkContext(HttpContextBase httpContext,
            ICustomerService customerService,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            TaxSettings taxSettings, CurrencySettings currencySettings,
            IWebHelper webHelper)
        {
            this._httpContext = httpContext;
            this._customerService = customerService;
            this._authenticationService = authenticationService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._taxSettings = taxSettings;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
        }

        protected Customer GetCurrentCustomer()
        {
            if (_cachedCustomer != null)
                return _cachedCustomer;

            Customer customer = null;
            if (_httpContext != null)
            {
                //registered user
                customer = _authenticationService.GetAuthenticatedCustomer();

                //impersonate user if required (currently used for 'phone order' support)
                //and validate that the current user is admin
                if (customer != null && !customer.Deleted && customer.Active)
                {
                    if (customer.IsAdmin()) 
                    {
                        int? impersonatedCustomerId = customer.GetAttribute<int?>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
                        if (impersonatedCustomerId.HasValue && impersonatedCustomerId.Value > 0)
                        {
                            var impersonatedCustomer = _customerService.GetCustomerById(impersonatedCustomerId.Value);
                            if (impersonatedCustomer != null && !impersonatedCustomer.Deleted && impersonatedCustomer.Active)
                            {
                                //set impersonated customer
                                _originalCustomerIfImpersonated = customer;
                                customer = impersonatedCustomer;
                            }
                        }
                    }
                }
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    var customerCookie = GetCustomerCookie();
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        var customerGuid = Guid.Empty;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid))
                        {
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            //this customer (from cookie) should not be registered
                            if (customerByCookie != null && !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                //load guest customer
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    var customerCookie = GetCustomerCookie();
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        var customerGuid = Guid.Empty;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid))
                        {
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            //this customer (from cookie) should not be registered
                            if (customerByCookie != null && !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    //TODO we should not create guest customer if request is made by search engine?
                    //perhaps, use a single built-in system record for search engines
                    //don't allow it to be registered
                    //ensure that it can't be loaded by GUID from cookie (below) - so noone can be impersonated as search engine
                    //ensure that it'll not be deleted in ICustomerService.DeleteGuestCustomers


                    customer = _customerService.InsertGuestCustomer();
                }

                SetCustomerCookie(customer.CustomerGuid);
            }

            //validation
            if (customer != null && !customer.Deleted && customer.Active)
            {
                //update last activity date
                if (customer.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
                {
                    customer.LastActivityDateUtc = DateTime.UtcNow;
                    _customerService.UpdateCustomer(customer);
                }

                //update IP address
                string currentIpAddress = _webHelper.GetCurrentIpAddress();
                if (!String.IsNullOrEmpty(currentIpAddress))
                {
                    if (!currentIpAddress.Equals(customer.LastIpAddress))
                    {
                        customer.LastIpAddress = currentIpAddress;
                        _customerService.UpdateCustomer(customer);
                    }
                }

                _cachedCustomer = customer;
            }

            return _cachedCustomer;
        }

        protected HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[CustomerCookieName];
        }

        protected void SetCustomerCookie(Guid customerGuid)
        {
            var cookie = new HttpCookie(CustomerCookieName);
            cookie.Value = customerGuid.ToString();
            if (customerGuid == Guid.Empty)
            {
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }
            else
            {
                int cookieExpires = 24 * 365; //TODO make configurable
                cookie.Expires = DateTime.Now.AddHours(cookieExpires);
            }
            if (_httpContext != null && _httpContext.Response != null)
            {
                _httpContext.Response.Cookies.Remove(CustomerCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public Customer CurrentCustomer
        {
            get
            {
                return GetCurrentCustomer();
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        public Customer OriginalCustomerIfImpersonated
        {
            get
            {
                return _originalCustomerIfImpersonated;
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                if (this.CurrentCustomer != null &&
                    this.CurrentCustomer.Language != null &&
                    this.CurrentCustomer.Language.Published)
                    return this.CurrentCustomer.Language;
                
                var lang = _languageService.GetAllLanguages().FirstOrDefault();
                return lang;
            }
            set
            {
                if (this.CurrentCustomer == null)
                    return;

                this.CurrentCustomer.Language = value;
                _customerService.UpdateCustomer(this.CurrentCustomer);
            }
        }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        public Currency WorkingCurrency
        {
            get
            {
                //return primary store currency when we're in admin area/mode
                if (this.IsAdmin)
                    return _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

                if (this.CurrentCustomer != null &&
                    this.CurrentCustomer.Currency != null &&
                    this.CurrentCustomer.Currency.Published)
                    return this.CurrentCustomer.Currency;

                var currency = _currencyService.GetAllCurrencies().FirstOrDefault();
                return currency;
            }
            set
            {
                if (this.CurrentCustomer == null)
                    return;

                this.CurrentCustomer.Currency = value;
                _customerService.UpdateCustomer(this.CurrentCustomer);
            }
        }

        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        public TaxDisplayType TaxDisplayType
        {
            get
            {
                if (_taxSettings.AllowCustomersToSelectTaxDisplayType)
                {
                    if (this.CurrentCustomer != null)
                        return this.CurrentCustomer.TaxDisplayType;
                }

                return _taxSettings.TaxDisplayType;
            }
            set
            {
                if (!_taxSettings.AllowCustomersToSelectTaxDisplayType)
                    return;

                this.CurrentCustomer.TaxDisplayType = value;
                _customerService.UpdateCustomer(this.CurrentCustomer);
            }
        }

        public bool IsAdmin
        {
            get
            {
                return _cachedIsAdmin;
            }
            set
            {
                _cachedIsAdmin = value;
            }
        }
    }
}
