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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Payment method manager
    /// </summary>
    public partial class PaymentMethodManager : IPaymentMethodManager
    {
        #region Constants
        private const string PAYMENTMETHODS_BY_ID_KEY = "Nop.paymentmethod.id-{0}";
        private const string PAYMENTMETHODS_PATTERN_KEY = "Nop.paymentmethod.";
        #endregion

        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public PaymentMethodManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        public void DeletePaymentMethod(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return;

            
            if (!_context.IsAttached(paymentMethod))
                _context.PaymentMethods.Attach(paymentMethod);
            _context.DeleteObject(paymentMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>Payment method</returns>
        public PaymentMethod GetPaymentMethodById(int paymentMethodId)
        {
            if (paymentMethodId == 0)
                return null;

            string key = string.Format(PAYMENTMETHODS_BY_ID_KEY, paymentMethodId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (PaymentMethod)obj2;
            }

            
            var query = from pm in _context.PaymentMethods
                        where pm.PaymentMethodId == paymentMethodId
                        select pm;
            var paymentMethod = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, paymentMethod);
            }
            return paymentMethod;
        }

        /// <summary>
        /// Gets a payment method
        /// </summary>
        /// <param name="systemKeyword">Payment method system keyword</param>
        /// <returns>Payment method</returns>
        public PaymentMethod GetPaymentMethodBySystemKeyword(string systemKeyword)
        {
            
            var query = from pm in _context.PaymentMethods
                        where pm.SystemKeyword == systemKeyword
                        select pm;
            var paymentMethod = query.FirstOrDefault();

            return paymentMethod;
        }

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <returns>Payment method collection</returns>
        public List<PaymentMethod> GetAllPaymentMethods()
        {
            return GetAllPaymentMethods(null);
        }

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier</param>
        /// <returns>Payment method collection</returns>
        public List<PaymentMethod> GetAllPaymentMethods(int? filterByCountryId)
        {
            bool showHidden = NopContext.Current.IsAdmin;

            return GetAllPaymentMethods(filterByCountryId, showHidden);
        }

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier</param>
        /// <param name="showHidden">A value indicating whether the not active payment methods should be load</param>
        /// <returns>Payment method collection</returns>
        public List<PaymentMethod> GetAllPaymentMethods(int? filterByCountryId, bool showHidden)
        {
            
            var paymentMethods = _context.Sp_PaymentMethodLoadAll(showHidden, filterByCountryId).ToList();
            return paymentMethods;
        }

        /// <summary>
        /// Inserts a payment method
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        public void InsertPaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException("paymentMethod");
            
            paymentMethod.Name = CommonHelper.EnsureNotNull(paymentMethod.Name);
            paymentMethod.Name = CommonHelper.EnsureMaximumLength(paymentMethod.Name, 100);
            paymentMethod.VisibleName = CommonHelper.EnsureNotNull(paymentMethod.VisibleName);
            paymentMethod.VisibleName = CommonHelper.EnsureMaximumLength(paymentMethod.VisibleName, 100);
            paymentMethod.Description = CommonHelper.EnsureNotNull(paymentMethod.Description);
            paymentMethod.Description = CommonHelper.EnsureMaximumLength(paymentMethod.Description, 4000);
            paymentMethod.ConfigureTemplatePath = CommonHelper.EnsureNotNull(paymentMethod.ConfigureTemplatePath);
            paymentMethod.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(paymentMethod.ConfigureTemplatePath, 500);
            paymentMethod.UserTemplatePath = CommonHelper.EnsureNotNull(paymentMethod.UserTemplatePath);
            paymentMethod.UserTemplatePath = CommonHelper.EnsureMaximumLength(paymentMethod.UserTemplatePath, 500);
            paymentMethod.ClassName = CommonHelper.EnsureNotNull(paymentMethod.ClassName);
            paymentMethod.ClassName = CommonHelper.EnsureMaximumLength(paymentMethod.ClassName, 500);
            paymentMethod.SystemKeyword = CommonHelper.EnsureNotNull(paymentMethod.SystemKeyword);
            paymentMethod.SystemKeyword = CommonHelper.EnsureMaximumLength(paymentMethod.SystemKeyword, 500);

            

            _context.PaymentMethods.AddObject(paymentMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the payment method
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        public void UpdatePaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException("paymentMethod");

            paymentMethod.Name = CommonHelper.EnsureNotNull(paymentMethod.Name);
            paymentMethod.Name = CommonHelper.EnsureMaximumLength(paymentMethod.Name, 100);
            paymentMethod.VisibleName = CommonHelper.EnsureNotNull(paymentMethod.VisibleName);
            paymentMethod.VisibleName = CommonHelper.EnsureMaximumLength(paymentMethod.VisibleName, 100);
            paymentMethod.Description = CommonHelper.EnsureNotNull(paymentMethod.Description);
            paymentMethod.Description = CommonHelper.EnsureMaximumLength(paymentMethod.Description, 4000);
            paymentMethod.ConfigureTemplatePath = CommonHelper.EnsureNotNull(paymentMethod.ConfigureTemplatePath);
            paymentMethod.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(paymentMethod.ConfigureTemplatePath, 500);
            paymentMethod.UserTemplatePath = CommonHelper.EnsureNotNull(paymentMethod.UserTemplatePath);
            paymentMethod.UserTemplatePath = CommonHelper.EnsureMaximumLength(paymentMethod.UserTemplatePath, 500);
            paymentMethod.ClassName = CommonHelper.EnsureNotNull(paymentMethod.ClassName);
            paymentMethod.ClassName = CommonHelper.EnsureMaximumLength(paymentMethod.ClassName, 500);
            paymentMethod.SystemKeyword = CommonHelper.EnsureNotNull(paymentMethod.SystemKeyword);
            paymentMethod.SystemKeyword = CommonHelper.EnsureMaximumLength(paymentMethod.SystemKeyword, 500);

            
            if (!_context.IsAttached(paymentMethod))
                _context.PaymentMethods.Attach(paymentMethod);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Creates the payment method country mapping
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        public void CreatePaymentMethodCountryMapping(int paymentMethodId, int countryId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return;

            var country = IoCFactory.Resolve<ICountryManager>().GetCountryById(countryId);
            if (country == null)
                return;

            
            if (!_context.IsAttached(paymentMethod))
                _context.PaymentMethods.Attach(paymentMethod);
            if (!_context.IsAttached(country))
                _context.Countries.Attach(country);

            //ensure that navigation property is loaded
            if (country.NpRestrictedPaymentMethods == null)
                _context.LoadProperty(country, c => c.NpRestrictedPaymentMethods);

            country.NpRestrictedPaymentMethods.Add(paymentMethod);
            _context.SaveChanges();
        }

        /// <summary>
        /// Checking whether the payment method country mapping exists
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>True if mapping exist, otherwise false</returns>
        public bool DoesPaymentMethodCountryMappingExist(int paymentMethodId, int countryId)
        {
            

            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return false;

            //ensure that navigation property is loaded
            if (paymentMethod.NpRestrictedCountries == null)
                _context.LoadProperty(paymentMethod, p => p.NpRestrictedCountries);

            bool result = paymentMethod.NpRestrictedCountries.ToList().Find(c => c.CountryId == countryId) != null;
            return result;

            //var query = from pm in _context.PaymentMethods
            //            from c in pm.NpRestrictedCountries
            //            where pm.PaymentMethodId == paymentMethodId &&
            //            c.CountryId == countryId
            //            select pm;

            //bool result = query.Count() > 0;
            //return result;
        }

        /// <summary>
        /// Deletes the payment method country mapping
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        public void DeletePaymentMethodCountryMapping(int paymentMethodId, int countryId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return;

            var country = IoCFactory.Resolve<ICountryManager>().GetCountryById(countryId);
            if (country == null)
                return;

            
            if (!_context.IsAttached(paymentMethod))
                _context.PaymentMethods.Attach(paymentMethod);
            if (!_context.IsAttached(country))
                _context.Countries.Attach(country);

            //ensure that navigation property is loaded
            if (country.NpRestrictedPaymentMethods == null)
                _context.LoadProperty(country, c => c.NpRestrictedPaymentMethods);

            country.NpRestrictedPaymentMethods.Remove(paymentMethod);
            _context.SaveChanges();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.PaymentMethodManager.CacheEnabled");
            }
        }
        #endregion
    }
}
