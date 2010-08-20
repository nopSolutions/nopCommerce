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

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Payment method manager
    /// </summary>
    public partial class PaymentMethodManager
    {
        #region Constants
        private const string PAYMENTMETHODS_BY_ID_KEY = "Nop.paymentmethod.id-{0}";
        private const string PAYMENTMETHODS_PATTERN_KEY = "Nop.paymentmethod.";
        #endregion

        #region Methods
        /// <summary>
        /// Deletes a payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        public static void DeletePaymentMethod(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(paymentMethod))
                context.PaymentMethods.Attach(paymentMethod);
            context.DeleteObject(paymentMethod);
            context.SaveChanges();

            if (PaymentMethodManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>Payment method</returns>
        public static PaymentMethod GetPaymentMethodById(int paymentMethodId)
        {
            if (paymentMethodId == 0)
                return null;

            string key = string.Format(PAYMENTMETHODS_BY_ID_KEY, paymentMethodId);
            object obj2 = NopRequestCache.Get(key);
            if (PaymentMethodManager.CacheEnabled && (obj2 != null))
            {
                return (PaymentMethod)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pm in context.PaymentMethods
                        where pm.PaymentMethodId == paymentMethodId
                        select pm;
            var paymentMethod = query.SingleOrDefault();

            if (PaymentMethodManager.CacheEnabled)
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
        public static PaymentMethod GetPaymentMethodBySystemKeyword(string systemKeyword)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from pm in context.PaymentMethods
                        where pm.SystemKeyword == systemKeyword
                        select pm;
            var paymentMethod = query.FirstOrDefault();

            return paymentMethod;
        }

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <returns>Payment method collection</returns>
        public static List<PaymentMethod> GetAllPaymentMethods()
        {
            return GetAllPaymentMethods(null);
        }

        /// <summary>
        /// Gets all payment methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier</param>
        /// <returns>Payment method collection</returns>
        public static List<PaymentMethod> GetAllPaymentMethods(int? filterByCountryId)
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
        public static List<PaymentMethod> GetAllPaymentMethods(int? filterByCountryId, bool showHidden)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var paymentMethods = context.Sp_PaymentMethodLoadAll(showHidden, filterByCountryId);
            return paymentMethods;
        }

        /// <summary>
        /// Inserts a payment method
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="visibleName">The visible name</param>
        /// <param name="description">The description</param>
        /// <param name="configureTemplatePath">The configure template path</param>
        /// <param name="userTemplatePath">The user template path</param>
        /// <param name="className">The class name</param>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="hidePaymentInfoForZeroOrders">A value indicating whether customers should provide their payment information with zero total orders</param>
        /// <param name="isActive">A value indicating whether the payment method is active</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Payment method</returns>
        public static PaymentMethod InsertPaymentMethod(string name,
            string visibleName, string description, string configureTemplatePath,
            string userTemplatePath, string className, string systemKeyword,
            bool hidePaymentInfoForZeroOrders, bool isActive, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            visibleName = CommonHelper.EnsureMaximumLength(visibleName, 100);
            description = CommonHelper.EnsureMaximumLength(description, 4000);
            configureTemplatePath = CommonHelper.EnsureMaximumLength(configureTemplatePath, 500);
            userTemplatePath = CommonHelper.EnsureMaximumLength(userTemplatePath, 500);
            className = CommonHelper.EnsureMaximumLength(className, 500);
            systemKeyword = CommonHelper.EnsureMaximumLength(systemKeyword, 500);

            var context = ObjectContextHelper.CurrentObjectContext;

            var paymentMethod = context.PaymentMethods.CreateObject();
            paymentMethod.Name = name;
            paymentMethod.VisibleName = visibleName;
            paymentMethod.Description = description;
            paymentMethod.ConfigureTemplatePath = configureTemplatePath;
            paymentMethod.UserTemplatePath = userTemplatePath;
            paymentMethod.ClassName = className;
            paymentMethod.SystemKeyword = systemKeyword;
            paymentMethod.HidePaymentInfoForZeroOrders = hidePaymentInfoForZeroOrders;
            paymentMethod.IsActive = isActive;
            paymentMethod.DisplayOrder = displayOrder;

            context.PaymentMethods.AddObject(paymentMethod);
            context.SaveChanges();

            if (PaymentMethodManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
            }
            return paymentMethod;
        }

        /// <summary>
        /// Updates the payment method
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifer</param>
        /// <param name="name">The name</param>
        /// <param name="visibleName">The visible name</param>
        /// <param name="description">The description</param>
        /// <param name="configureTemplatePath">The configure template path</param>
        /// <param name="userTemplatePath">The user template path</param>
        /// <param name="className">The class name</param>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="hidePaymentInfoForZeroOrders">A value indicating whether customers should provide their payment information with zero total orders</param>
        /// <param name="isActive">A value indicating whether the payment method is active</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Payment method</returns>
        public static PaymentMethod UpdatePaymentMethod(int paymentMethodId,
            string name, string visibleName, string description, string configureTemplatePath,
            string userTemplatePath, string className, string systemKeyword,
            bool hidePaymentInfoForZeroOrders, bool isActive, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            visibleName = CommonHelper.EnsureMaximumLength(visibleName, 100);
            description = CommonHelper.EnsureMaximumLength(description, 4000);
            configureTemplatePath = CommonHelper.EnsureMaximumLength(configureTemplatePath, 500);
            userTemplatePath = CommonHelper.EnsureMaximumLength(userTemplatePath, 500);
            className = CommonHelper.EnsureMaximumLength(className, 500);
            systemKeyword = CommonHelper.EnsureMaximumLength(systemKeyword, 500);

            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(paymentMethod))
                context.PaymentMethods.Attach(paymentMethod);

            paymentMethod.Name = name;
            paymentMethod.VisibleName = visibleName;
            paymentMethod.Description = description;
            paymentMethod.ConfigureTemplatePath = configureTemplatePath;
            paymentMethod.UserTemplatePath = userTemplatePath;
            paymentMethod.ClassName = className;
            paymentMethod.SystemKeyword = systemKeyword;
            paymentMethod.HidePaymentInfoForZeroOrders = hidePaymentInfoForZeroOrders;
            paymentMethod.IsActive = isActive;
            paymentMethod.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (PaymentMethodManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
            }
            return paymentMethod;
        }

        /// <summary>
        /// Creates the payment method country mapping
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        public static void CreatePaymentMethodCountryMapping(int paymentMethodId, int countryId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return;

            var country = CountryManager.GetCountryById(countryId);
            if (country == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(paymentMethod))
                context.PaymentMethods.Attach(paymentMethod);
            if (!context.IsAttached(country))
                context.Countries.Attach(country);

            country.NpRestrictedPaymentMethods.Add(paymentMethod);
            context.SaveChanges();
        }

        /// <summary>
        /// Checking whether the payment method country mapping exists
        /// </summary>
        /// <param name="paymentMethodId">The payment method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>True if mapping exist, otherwise false</returns>
        public static bool DoesPaymentMethodCountryMappingExist(int paymentMethodId, int countryId)
        {
            var context = ObjectContextHelper.CurrentObjectContext;

            var pm = GetPaymentMethodById(paymentMethodId);
            bool result = pm.NpRestrictedCountries.ToList().Find(c => c.CountryId == countryId) != null;
            return result;

            //var query = from pm in context.PaymentMethods
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
        public static void DeletePaymentMethodCountryMapping(int paymentMethodId, int countryId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return;

            var country = CountryManager.GetCountryById(countryId);
            if (country == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(paymentMethod))
                context.PaymentMethods.Attach(paymentMethod);
            if (!context.IsAttached(country))
                context.Countries.Attach(country);

            country.NpRestrictedPaymentMethods.Remove(paymentMethod);
            context.SaveChanges();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.PaymentMethodManager.CacheEnabled");
            }
        }
        #endregion
    }
}
