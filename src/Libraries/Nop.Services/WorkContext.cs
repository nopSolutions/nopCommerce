//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------


using System;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
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
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;

        private Customer _cachedCustomer;
        private string _workingTheme = "";

        public WorkContext(HttpContextBase httpContext,
            ICustomerService customerService,
            ILanguageService languageService,
            ICurrencyService currencyService)
        {
            this._httpContext = httpContext;
            this._customerService = customerService;
            this._languageService = languageService;
            this._currencyService = currencyService;
        }

        protected Customer GetCurrentCustomer()
        {
            if (_cachedCustomer != null)
                return _cachedCustomer;

            Customer customer = null;
            if (_httpContext != null)
            {
                var customerCookie = GetCustomerCookie();

                if (_httpContext.User != null &&
                    _httpContext.User.Identity != null &&
                    _httpContext.User.Identity.IsAuthenticated)
                {
                    customer = _customerService.GetCustomerByUsername(_httpContext.User.Identity.Name);
                }
                else if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                {
                    var customerGuid = Guid.Empty;
                    if (Guid.TryParse(customerCookie.Value, out customerGuid))
                        customer = _customerService.GetCustomerByGuid(customerGuid);
                }


                if (customer == null)
                {
                    customer = _customerService.InsertGuestCustomer(Guid.NewGuid().ToString());
                }

                SetCustomerCookie(customer.CustomerGuid);
            }

            _cachedCustomer = customer;
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
            //TODO encrypt customer GUID

            var cookie = new HttpCookie(CustomerCookieName);
            cookie.Value = customerGuid.ToString();
            if (customerGuid == Guid.Empty)
            {
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }
            else
            {
                int cookieExpires = 24 * 365; //TODO make confgiurable
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
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                if (this.CurrentCustomer == null)
                    return null;
                
                if (this.CurrentCustomer.Language != null &&
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
                if (this.CurrentCustomer == null)
                    return null;

                if (this.CurrentCustomer.Currency != null &&
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
        /// Get or set current theme (e.g. darkOrange)
        /// </summary>
        public string WorkingTheme
        {
            get
            {
                return _workingTheme;
            }
            set
            {
                _workingTheme = value;
            }
        }
    }
}
