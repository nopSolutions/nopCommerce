using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a payment plugin manager implementation
    /// </summary>
    public partial class PaymentPluginManager : PluginManager<IPaymentMethod>, IPaymentPluginManager
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly PaymentSettings _paymentSettings;

        #endregion

        #region Ctor

        public PaymentPluginManager(ICustomerService customerService,
            IPluginService pluginService,
            ISettingService settingService,
            PaymentSettings paymentSettings) : base(customerService, pluginService)
        {
            _settingService = settingService;
            _paymentSettings = paymentSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active payment methods
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="countryId">Filter by country; pass 0 to load all plugins</param>
        /// <returns>List of active payment methods</returns>
        public virtual IList<IPaymentMethod> LoadActivePlugins(Customer customer = null, int storeId = 0, int countryId = 0)
        {
            var paymentMethods = LoadActivePlugins(_paymentSettings.ActivePaymentMethodSystemNames, customer, storeId);

            //filter by country
            if (countryId > 0)
                paymentMethods = paymentMethods.Where(method => !GetRestrictedCountryIds(method).Contains(countryId)).ToList();

            return paymentMethods;
        }

        /// <summary>
        /// Check whether the passed payment method is active
        /// </summary>
        /// <param name="paymentMethod">Payment method to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IPaymentMethod paymentMethod)
        {
            return IsPluginActive(paymentMethod, _paymentSettings.ActivePaymentMethodSystemNames);
        }

        /// <summary>
        /// Check whether the payment method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of payment method to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, Customer customer = null, int storeId = 0)
        {
            var paymentMethod = LoadPluginBySystemName(systemName, customer, storeId);
            return IsPluginActive(paymentMethod);
        }

        /// <summary>
        /// Get countries in which the passed payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>List of country identifiers</returns>
        public virtual IList<int> GetRestrictedCountryIds(IPaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            var settingKey = string.Format(NopPaymentDefaults.RestrictedCountriesSettingName, paymentMethod.PluginDescriptor.SystemName);

            return _settingService.GetSettingByKey<List<int>>(settingKey) ?? new List<int>();
        }

        /// <summary>
        /// Save countries in which the passed payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="countryIds">List of country identifiers</param>
        public virtual void SaveRestrictedCountries(IPaymentMethod paymentMethod, IList<int> countryIds)
        {
            if (paymentMethod == null)
                throw new ArgumentNullException(nameof(paymentMethod));

            var settingKey = string.Format(NopPaymentDefaults.RestrictedCountriesSettingName, paymentMethod.PluginDescriptor.SystemName);

            _settingService.SetSetting(settingKey, countryIds.ToList());
        }

        #endregion
    }
}