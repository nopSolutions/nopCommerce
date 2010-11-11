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
using System.Linq;
using System.Text;
using System.Web.Compilation;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Localization;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Payment service
    /// </summary>
    public partial class PaymentService : IPaymentService
    {
        #region Constants
        private const string CREDITCARDS_ALL_KEY = "Nop.creditcard.all";
        private const string CREDITCARDS_BY_ID_KEY = "Nop.creditcard.id-{0}";
        private const string CREDITCARDS_PATTERN_KEY = "Nop.creditcard.";

        private const string PAYMENTMETHODS_BY_ID_KEY = "Nop.paymentmethod.id-{0}";
        private const string PAYMENTMETHODS_PATTERN_KEY = "Nop.paymentmethod.";

        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public PaymentService(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        #region Credit cards
        /// <summary>
        /// Gets a credit card type
        /// </summary>
        /// <param name="creditCardTypeId">Credit card type identifier</param>
        /// <returns>Credit card type</returns>
        public CreditCardType GetCreditCardTypeById(int creditCardTypeId)
        {
            if (creditCardTypeId == 0)
                return null;

            string key = string.Format(CREDITCARDS_BY_ID_KEY, creditCardTypeId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (CreditCardType)obj2;
            }


            var query = from cct in _context.CreditCardTypes
                        where cct.CreditCardTypeId == creditCardTypeId
                        select cct;
            var creditCardType = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, creditCardType);
            }
            return creditCardType;
        }

        /// <summary>
        /// Marks a credit card type as deleted
        /// </summary>
        /// <param name="creditCardTypeId">Credit card type identifier</param>
        public void MarkCreditCardTypeAsDeleted(int creditCardTypeId)
        {
            var creditCardType = GetCreditCardTypeById(creditCardTypeId);
            if (creditCardType != null)
            {
                creditCardType.Deleted = true;
                UpdateCreditCardType(creditCardType);
            }
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CREDITCARDS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all credit card types
        /// </summary>
        /// <returns>Credit card type collection</returns>
        public List<CreditCardType> GetAllCreditCardTypes()
        {
            string key = string.Format(CREDITCARDS_ALL_KEY);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CreditCardType>)obj2;
            }


            var query = from cct in _context.CreditCardTypes
                        orderby cct.DisplayOrder
                        where !cct.Deleted
                        select cct;
            var creditCardTypeCollection = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, creditCardTypeCollection);
            }
            return creditCardTypeCollection;
        }

        /// <summary>
        /// Inserts a credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        public void InsertCreditCardType(CreditCardType creditCardType)
        {
            if (creditCardType == null)
                throw new ArgumentNullException("creditCardType");

            creditCardType.Name = CommonHelper.EnsureNotNull(creditCardType.Name);
            creditCardType.Name = CommonHelper.EnsureMaximumLength(creditCardType.Name, 100);
            creditCardType.SystemKeyword = CommonHelper.EnsureNotNull(creditCardType.SystemKeyword);
            creditCardType.SystemKeyword = CommonHelper.EnsureMaximumLength(creditCardType.SystemKeyword, 100);



            _context.CreditCardTypes.AddObject(creditCardType);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CREDITCARDS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        public void UpdateCreditCardType(CreditCardType creditCardType)
        {
            if (creditCardType == null)
                throw new ArgumentNullException("creditCardType");

            creditCardType.Name = CommonHelper.EnsureNotNull(creditCardType.Name);
            creditCardType.Name = CommonHelper.EnsureMaximumLength(creditCardType.Name, 100);
            creditCardType.SystemKeyword = CommonHelper.EnsureNotNull(creditCardType.SystemKeyword);
            creditCardType.SystemKeyword = CommonHelper.EnsureMaximumLength(creditCardType.SystemKeyword, 100);


            if (!_context.IsAttached(creditCardType))
                _context.CreditCardTypes.Attach(creditCardType);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(CREDITCARDS_PATTERN_KEY);
            }
        }
        #endregion

        #region Payment methods

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
                _cacheManager.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
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
            object obj2 = _cacheManager.Get(key);
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
                _cacheManager.Add(key, paymentMethod);
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
                _cacheManager.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
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
                _cacheManager.RemoveByPattern(PAYMENTMETHODS_PATTERN_KEY);
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

            var country = IoC.Resolve<ICountryService>().GetCountryById(countryId);
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

            var country = IoC.Resolve<ICountryService>().GetCountryById(countryId);
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
                
        #region Workflow

        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessPayment(PaymentInfo paymentInfo, Customer customer, 
            Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            if (paymentInfo.OrderTotal == decimal.Zero)
            {
                processPaymentResult.Error = string.Empty;
                processPaymentResult.FullError = string.Empty;
                processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
            }
            else
            {
                var paymentMethod = GetPaymentMethodById(paymentInfo.PaymentMethodId);
                if (paymentMethod == null)
                    throw new NopException("Payment method couldn't be loaded");
                var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
                iPaymentMethod.ProcessPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
            }
        }

        /// <summary>
        /// Post process payment (payment gateways that require redirecting)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>The error status, or String.Empty if no errors</returns>
        public string PostProcessPayment(Order order)
        {
            //already paid or order.OrderTotal == decimal.Zero
            if (order.PaymentStatus == PaymentStatusEnum.Paid)
                return string.Empty;

            var paymentMethod = GetPaymentMethodById(order.PaymentMethodId);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            return iPaymentMethod.PostProcessPayment(order);
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return decimal.Zero;
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;

            decimal result = iPaymentMethod.GetAdditionalHandlingFee();
            if (result < decimal.Zero)
                result = decimal.Zero;
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether capture is supported</returns>
        public bool CanCapture(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return false;
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            return iPaymentMethod.CanCapture;
        }
        
        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void Capture(Order order, ref ProcessPaymentResult processPaymentResult)
        {
            var paymentMethod = GetPaymentMethodById(order.PaymentMethodId);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            iPaymentMethod.Capture(order, ref processPaymentResult);
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether partial refund is supported</returns>
        public bool CanPartiallyRefund(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return false;
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            return iPaymentMethod.CanPartiallyRefund;
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether refund is supported</returns>
        public bool CanRefund(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return false;
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            return iPaymentMethod.CanRefund;
        }

        /// <summary>
        /// Refunds payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        public void Refund(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            var paymentMethod = GetPaymentMethodById(order.PaymentMethodId);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            iPaymentMethod.Refund(order, ref cancelPaymentResult);
        }

        /// <summary>
        /// Gets a value indicating whether void is supported by payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A value indicating whether void is supported</returns>
        public bool CanVoid(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return false;
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            return iPaymentMethod.CanVoid;
        }

        /// <summary>
        /// Voids payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        public void Void(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            var paymentMethod = GetPaymentMethodById(order.PaymentMethodId);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            iPaymentMethod.Void(order, ref cancelPaymentResult);
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A recurring payment type of payment method</returns>
        public RecurringPaymentTypeEnum SupportRecurringPayments(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return RecurringPaymentTypeEnum.NotSupported;
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            return iPaymentMethod.SupportRecurringPayments;
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <returns>A payment method type</returns>
        public PaymentMethodTypeEnum GetPaymentMethodType(int paymentMethodId)
        {
            var paymentMethod = GetPaymentMethodById(paymentMethodId);
            if (paymentMethod == null)
                return PaymentMethodTypeEnum.Unknown;
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            return iPaymentMethod.PaymentMethodType;
        }

        /// <summary>
        /// Process recurring payments
        /// </summary>
        /// <param name="paymentInfo">Payment info required for an order processing</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderGuid">Unique order identifier</param>
        /// <param name="processPaymentResult">Process payment result</param>
        public void ProcessRecurringPayment(PaymentInfo paymentInfo, Customer customer,
            Guid orderGuid, ref ProcessPaymentResult processPaymentResult)
        {
            if (paymentInfo.OrderTotal == decimal.Zero)
            {
                processPaymentResult.Error = string.Empty;
                processPaymentResult.FullError = string.Empty;
                processPaymentResult.PaymentStatus = PaymentStatusEnum.Paid;
            }
            else
            {
                var paymentMethod = GetPaymentMethodById(paymentInfo.PaymentMethodId);
                if (paymentMethod == null)
                    throw new NopException("Payment method couldn't be loaded");
                var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
                iPaymentMethod.ProcessRecurringPayment(paymentInfo, customer, orderGuid, ref processPaymentResult);
            }
        }

        /// <summary>
        /// Cancels recurring payment
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="cancelPaymentResult">Cancel payment result</param>
        public void CancelRecurringPayment(Order order, ref CancelPaymentResult cancelPaymentResult)
        {
            if (order.OrderTotal == decimal.Zero)
                return;

            var paymentMethod = GetPaymentMethodById(order.PaymentMethodId);
            if (paymentMethod == null)
                throw new NopException("Payment method couldn't be loaded");
            var iPaymentMethod = Activator.CreateInstance(Type.GetType(paymentMethod.ClassName)) as IPaymentMethod;
            iPaymentMethod.CancelRecurringPayment(order, ref cancelPaymentResult);
        }

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        public string GetMaskedCreditCardNumber(string creditCardNumber)
        {
            if (String.IsNullOrEmpty(creditCardNumber))
                return string.Empty;

            if (creditCardNumber.Length <= 4)
                return creditCardNumber;

            string last4 = creditCardNumber.Substring(creditCardNumber.Length - 4, 4);
            string maskedChars = string.Empty;
            for (int i = 0; i < creditCardNumber.Length - 4; i++)
            {
                maskedChars += "*";
            }
            return maskedChars + last4;
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.PaymentManager.CacheEnabled");
            }
        }
        #endregion
    }
}
