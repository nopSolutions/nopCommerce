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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using System.Collections.Generic;

namespace NopSolutions.NopCommerce.BusinessLogic
{
    /// <summary>
    /// Represents a NopContext
    /// </summary>
    public partial class NopContext
    {
        #region Constants
        private const string CONST_CUSTOMERSESSION = "Nop.CustomerSession";
        private const string CONST_CUSTOMERSESSIONCOOKIE = "Nop.CustomerSessionGUIDCookie";
        #endregion

        #region Fields
        private Customer _currentCustomer;
        private bool _isCurrentCustomerImpersonated;
        private Customer _originalCustomer;
        private bool? _isAdmin;
        private HttpContext _context = HttpContext.Current;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the NopContext class
        /// </summary>
        private NopContext()
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Save customer session to data source
        /// </summary>
        /// <returns>Saved customer ssion</returns>
        private CustomerSession SaveSessionToDatabase()
        {
            var sessionId = Guid.NewGuid();
            while (CustomerManager.GetCustomerSessionByGuid(sessionId) != null)
                sessionId = Guid.NewGuid();
            var session = new CustomerSession();
            int CustomerId = 0;
            if (this.User != null)
            {
                CustomerId = this.User.CustomerId;
            }
            session.CustomerSessionGuid = sessionId;
            session.CustomerId = CustomerId;
            session.LastAccessed = DateTime.UtcNow;
            session.IsExpired = false;
            session = CustomerManager.SaveCustomerSession(session.CustomerSessionGuid, session.CustomerId, session.LastAccessed, session.IsExpired);
            return session;
        }

        /// <summary>
        /// Gets customer session
        /// </summary>
        /// <param name="createInDatabase">Create session in database if no one exists</param>
        /// <returns>Customer session</returns>
        public CustomerSession GetSession(bool createInDatabase)
        {
            return this.GetSession(createInDatabase, null);
        }

        /// <summary>
        /// Gets customer session
        /// </summary>
        /// <param name="createInDatabase">Create session in database if no one exists</param>
        /// <param name="sessionId">Session identifier</param>
        /// <returns>Customer session</returns>
        public CustomerSession GetSession(bool createInDatabase, Guid? sessionId)
        {
            CustomerSession byId = null;
            object obj2 = Current[CONST_CUSTOMERSESSION];
            if (obj2 != null)
                byId = (CustomerSession)obj2;
            if ((byId == null) && (sessionId.HasValue))
            {
                byId = CustomerManager.GetCustomerSessionByGuid(sessionId.Value);
                return byId;
            }
            if (byId == null && createInDatabase)
            {
                byId = SaveSessionToDatabase();
            }
            string customerSessionCookieValue = string.Empty;
            if ((HttpContext.Current.Request.Cookies[CONST_CUSTOMERSESSIONCOOKIE] != null) && (HttpContext.Current.Request.Cookies[CONST_CUSTOMERSESSIONCOOKIE].Value != null))
                customerSessionCookieValue = HttpContext.Current.Request.Cookies[CONST_CUSTOMERSESSIONCOOKIE].Value;
            if ((byId) == null && (!string.IsNullOrEmpty(customerSessionCookieValue)))
            {
                var dbCustomerSession = CustomerManager.GetCustomerSessionByGuid(new Guid(customerSessionCookieValue));
                byId = dbCustomerSession;
            }
            Current[CONST_CUSTOMERSESSION] = byId;
            return byId;
        }

        /// <summary>
        /// Saves current session to client
        /// </summary>
        public void SessionSaveToClient()
        {
            if (HttpContext.Current != null && this.Session != null)
                SetCookie(HttpContext.Current.ApplicationInstance, CONST_CUSTOMERSESSIONCOOKIE, this.Session.CustomerSessionGuid.ToString());
        }

        /// <summary>
        /// Reset customer session
        /// </summary>
        public void ResetSession()
        {
            if (HttpContext.Current != null)
                SetCookie(HttpContext.Current.ApplicationInstance, CONST_CUSTOMERSESSIONCOOKIE, string.Empty);
            this.Session = null;
            this.User = null;
            this["Nop.SessionReseted"] = true;
        }

        /// <summary>
        /// Sets cookie
        /// </summary>
        /// <param name="application">Application</param>
        /// <param name="key">Key</param>
        /// <param name="val">Value</param>
        private static void SetCookie(HttpApplication application, string key, string val)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Value = val;
            if (string.IsNullOrEmpty(val))
            {
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }
            else
            {
                cookie.Expires = DateTime.Now.AddHours((double)NopConfig.CookieExpires);
            }
            application.Response.Cookies.Remove(key);
            application.Response.Cookies.Add(cookie);
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the NopContext, which can be used to retrieve information about current context.
        /// </summary>
        public static NopContext Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    object data = Thread.GetData(Thread.GetNamedDataSlot("NopContext"));
                    if (data != null)
                    {
                        return (NopContext)data;
                    }
                    NopContext context = new NopContext();
                    Thread.SetData(Thread.GetNamedDataSlot("NopContext"), context);
                    return context;
                }
                if (HttpContext.Current.Items["NopContext"] == null)
                {
                    NopContext context = new NopContext();
                    HttpContext.Current.Items.Add("NopContext", context);
                    return context;
                }
                return (NopContext)HttpContext.Current.Items["NopContext"];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the context is running in admin-mode
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                if (!_isAdmin.HasValue)
                {
                    _isAdmin = CommonHelper.IsAdmin();
                }
                return _isAdmin.Value;
            }
            set
            {
                _isAdmin = value;
            }
        }

        /// <summary>
        /// Gets or sets an object item in the context by the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public object this[string key]
        {
            get
            {
                if (this._context == null)
                {
                    return null;
                }

                if (this._context.Items[key] != null)
                {
                    return this._context.Items[key];
                }
                return null;
            }
            set
            {
                if (this._context != null)
                {
                    this._context.Items.Remove(key);
                    this._context.Items.Add(key, value);

                }
            }
        }

        /// <summary>
        /// Gets or sets the current session
        /// </summary>
        public CustomerSession Session
        {
            get
            {
                return this.GetSession(false);
            }
            set
            {
                Current[CONST_CUSTOMERSESSION] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public Customer User
        {
            get
            {
                return this._currentCustomer;
            }
            set
            {
                this._currentCustomer = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether current user is impersonated
        /// </summary>
        public bool IsCurrentCustomerImpersonated
        {
            get
            {
                return this._isCurrentCustomerImpersonated;
            }
            set
            {
                this._isCurrentCustomerImpersonated = value;
            }
        }

        /// <summary>
        /// Gets or sets the current user (in case th current user is impersonated)
        /// </summary>
        public Customer OriginalUser
        {
            get
            {
                return this._originalCustomer;
            }
            set
            {
                this._originalCustomer = value;
            }
        }

        /// <summary>
        /// Gets an user host address
        /// </summary>
        public string UserHostAddress
        {
            get
            {
                if (HttpContext.Current != null &&
                    HttpContext.Current.Request != null &&
                    HttpContext.Current.Request.UserHostAddress != null)
                    return HttpContext.Current.Request.UserHostAddress;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        public Currency WorkingCurrency
        {
            get
            {
                if (NopContext.Current == null || NopContext.Current.IsAdmin)
                {
                    return CurrencyManager.PrimaryStoreCurrency;
                }
                var customer = NopContext.Current.User;
                var publishedCurrencies = CurrencyManager.GetAllCurrencies();
                if (customer != null)
                {
                    var customerCurrency = customer.Currency;
                    if (customerCurrency != null)
                        foreach (Currency _currency in publishedCurrencies)
                            if (_currency.CurrencyId == customerCurrency.CurrencyId)
                                return customerCurrency;
                }
                else if (CommonHelper.GetCookieInt("Nop.CustomerCurrency") > 0)
                {
                    var customerCurrency = CurrencyManager.GetCurrencyById(CommonHelper.GetCookieInt("Nop.CustomerCurrency"));
                    if (customerCurrency != null)
                        foreach (Currency _currency in publishedCurrencies)
                            if (_currency.CurrencyId == customerCurrency.CurrencyId)
                                return customerCurrency;
                }
                foreach (var _currency in publishedCurrencies)
                    return _currency;
                return CurrencyManager.PrimaryStoreCurrency;
            }
            set
            {
                if (value == null)
                    return;
                var customer = NopContext.Current.User;
                if (customer != null)
                {
                    customer = CustomerManager.UpdateCustomer(customer.CustomerId, customer.CustomerGuid,
                        customer.Email, customer.Username, customer.PasswordHash,
                        customer.SaltKey, customer.AffiliateId,
                        customer.BillingAddressId, customer.ShippingAddressId,
                        customer.LastPaymentMethodId, customer.LastAppliedCouponCode,
                        customer.GiftCardCouponCodes, customer.CheckoutAttributes,
                        customer.LanguageId, value.CurrencyId, customer.TaxDisplayType,
                        customer.IsTaxExempt, customer.IsAdmin, customer.IsGuest,
                        customer.IsForumModerator, customer.TotalForumPosts,
                        customer.Signature, customer.AdminComment, customer.Active,
                        customer.Deleted, customer.RegistrationDate, 
                        customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);

                    NopContext.Current.User = customer;
                }
                if (!NopContext.Current.IsAdmin)
                {
                    CommonHelper.SetCookie("Nop.CustomerCurrency", value.CurrencyId.ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                var customer = NopContext.Current.User;
                if (customer != null)
                {
                    var customerLanguage = customer.Language;
                    if (customerLanguage != null && customerLanguage.Published)
                        return customerLanguage;
                }
                else if (CommonHelper.GetCookieInt("Nop.CustomerLanguage") > 0)
                {
                    var customerLanguage = LanguageManager.GetLanguageById(CommonHelper.GetCookieInt("Nop.CustomerLanguage"));
                    if (customerLanguage != null)
                        if (customerLanguage != null && customerLanguage.Published)
                            return customerLanguage;
                }
                var publishedLanguages = LanguageManager.GetAllLanguages(false);
                foreach (var _language in publishedLanguages)
                    return _language;

                throw new NopException("Languages could not be loaded");
            }
            set
            {
                if (value == null)
                    return;
                var customer = NopContext.Current.User;
                if (customer != null)
                {
                    customer = CustomerManager.UpdateCustomer(customer.CustomerId, 
                        customer.CustomerGuid, customer.Email, customer.Username, 
                        customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                        customer.BillingAddressId, customer.ShippingAddressId,
                        customer.LastPaymentMethodId, customer.LastAppliedCouponCode,
                        customer.GiftCardCouponCodes, customer.CheckoutAttributes, 
                        value.LanguageId, customer.CurrencyId, customer.TaxDisplayType,
                        customer.IsTaxExempt, customer.IsAdmin, customer.IsGuest,
                        customer.IsForumModerator, customer.TotalForumPosts,
                        customer.Signature, customer.AdminComment, customer.Active, customer.Deleted, customer.RegistrationDate,
                        customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);

                    NopContext.Current.User = customer;
                }

                CommonHelper.SetCookie("Nop.CustomerLanguage", value.LanguageId.ToString(), new TimeSpan(365, 0, 0, 0, 0));
            }
        }

        /// <summary>
        /// Get or set current theme (e.g. darkOrange)
        /// </summary>
        public string WorkingTheme
        {
            get
            {
                if (SettingManager.GetSettingValueBoolean("Display.AllowCustomerSelectTheme"))
                {
                    string themeCookie = CommonHelper.GetCookieString("Nop.WorkingTheme", false);
                    if (!String.IsNullOrEmpty(themeCookie))
                    {
                        //validate whether folder theme physically exists
                        string[] systemThemes = SettingManager.GetSettingValue("Display.SystemThemes").Split(',');
                        var tmp1 = from f in System.IO.Directory.GetDirectories(HttpContext.Current.Request.PhysicalApplicationPath + "App_Themes")
                                        where !systemThemes.Contains(System.IO.Path.GetFileName(f).ToLower())
                                        && themeCookie.Equals(System.IO.Path.GetFileName(f), StringComparison.InvariantCultureIgnoreCase)
                                        select System.IO.Path.GetFileName(f);
                        if (tmp1.ToList().Count > 0)
                            return themeCookie;
                    }
                }
                string defaultTheme = SettingManager.GetSettingValue("Display.PublicStoreTheme");
                if (!String.IsNullOrEmpty(defaultTheme))
                {
                    return defaultTheme;
                }
                return string.Empty;
            }
            set
            {
                if (!SettingManager.GetSettingValueBoolean("Display.AllowCustomerSelectTheme"))
                    return;

                CommonHelper.SetCookie("Nop.WorkingTheme", value.Trim(), new TimeSpan(365, 0, 0, 0, 0));
            }
        }

        /// <summary>
        /// Get a value indicating whether we have localized entity properties
        /// </summary>
        public bool LocalizedEntityPropertiesEnabled
        {
            get
            {
                bool showHidden = NopContext.Current.IsAdmin;
                var languages = LanguageManager.GetAllLanguages(showHidden);

                bool result = languages.Count > 1;
                return result;
            }
        }

        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        public TaxDisplayTypeEnum TaxDisplayType
        {
            get
            {
                //if (NopContext.Current.IsAdmin)
                //{
                //    return TaxManager.TaxDisplayType;
                //}

                if (TaxManager.AllowCustomersToSelectTaxDisplayType)
                {
                    var customer = NopContext.Current.User;
                    if (customer != null)
                    {
                        return customer.TaxDisplayType;
                    }
                    else if (CommonHelper.GetCookieInt("Nop.TaxDisplayTypeId") > 0)
                    {
                        return (TaxDisplayTypeEnum)Enum.ToObject(typeof(TaxDisplayTypeEnum), CommonHelper.GetCookieInt("Nop.TaxDisplayTypeId"));
                    }
                }
                return TaxManager.TaxDisplayType;
            }
            set
            {
                if (!TaxManager.AllowCustomersToSelectTaxDisplayType)
                    return;

                var customer = NopContext.Current.User;
                if (customer != null)
                {
                    customer = CustomerManager.UpdateCustomer(customer.CustomerId, customer.CustomerGuid,
                        customer.Email, customer.Username, customer.PasswordHash, customer.SaltKey, customer.AffiliateId,
                        customer.BillingAddressId, customer.ShippingAddressId,
                        customer.LastPaymentMethodId, customer.LastAppliedCouponCode,
                        customer.GiftCardCouponCodes, customer.CheckoutAttributes, 
                        customer.LanguageId, customer.CurrencyId, value,
                        customer.IsTaxExempt, customer.IsAdmin, customer.IsGuest,
                        customer.IsForumModerator, customer.TotalForumPosts,
                        customer.Signature, customer.AdminComment,
                        customer.Active, customer.Deleted, customer.RegistrationDate, 
                        customer.TimeZoneId, customer.AvatarId, customer.DateOfBirth);
                }
                if (!NopContext.Current.IsAdmin)
                {
                    CommonHelper.SetCookie("Nop.TaxDisplayTypeId", ((int)value).ToString(), new TimeSpan(365, 0, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// Gets the last page for "Continue shopping" button on shopping cart page
        /// </summary>
        public string LastContinueShoppingPage
        {
            get
            {
                if ((HttpContext.Current.Session != null) && (HttpContext.Current.Session["Nop.LastContinueShoppingPage"] != null))
                {
                    return HttpContext.Current.Session["Nop.LastContinueShoppingPage"].ToString();
                }
                return string.Empty;
            }
            set
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session["Nop.LastContinueShoppingPage"] = value;
                }
            }
        }

        /// <summary>
        /// Sets the CultureInfo 
        /// </summary>
        /// <param name="culture">Culture</param>
        public void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }
        
        #endregion
    }
}
