using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Shipping.Pickup
{
    /// <summary>
    /// Represents a pickup point plugin manager implementation
    /// </summary>
    public partial class PickupPluginManager : PluginManager<IPickupPointProvider>, IPickupPluginManager
    {
        #region Fields

        private readonly ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public PickupPluginManager(ICustomerService customerService,
            IPluginService pluginService,
            ShippingSettings shippingSettings) : base(customerService, pluginService)
        {
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active pickup point providers
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by pickup point provider system name; pass null to load all plugins</param>
        /// <returns>List of active pickup point providers</returns>
        public virtual IList<IPickupPointProvider> LoadActivePlugins(Customer customer = null, int storeId = 0, string systemName = null)
        {
            var pickupPointProviders = LoadActivePlugins(_shippingSettings.ActivePickupPointProviderSystemNames, customer, storeId);

            //filter by passed system name
            if (!string.IsNullOrEmpty(systemName))
            {
                pickupPointProviders = pickupPointProviders
                    .Where(provider => provider.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            return pickupPointProviders;
        }

        /// <summary>
        /// Check whether the passed pickup point provider is active
        /// </summary>
        /// <param name="pickupPointProvider">Pickup point provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IPickupPointProvider pickupPointProvider)
        {
            return IsPluginActive(pickupPointProvider, _shippingSettings.ActivePickupPointProviderSystemNames);
        }

        /// <summary>
        /// Check whether the pickup point provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of pickup point provider to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, Customer customer = null, int storeId = 0)
        {
            var pickupPointProvider = LoadPluginBySystemName(systemName, customer, storeId);
            return IsPluginActive(pickupPointProvider);
        }

        #endregion
    }
}