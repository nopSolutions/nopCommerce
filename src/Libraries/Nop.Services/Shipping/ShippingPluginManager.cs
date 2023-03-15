using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Represents a shipping plugin manager implementation
    /// </summary>
    public partial class ShippingPluginManager : PluginManager<IShippingRateComputationMethod>, IShippingPluginManager
    {
        #region Fields

        protected readonly ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public ShippingPluginManager(ICustomerService customerService,
            IPluginService pluginService,
            ShippingSettings shippingSettings) : base(customerService, pluginService)
        {
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active shipping providers
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by shipping provider system name; pass null to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active shipping providers
        /// </returns>
        public virtual async Task<IList<IShippingRateComputationMethod>> LoadActivePluginsAsync(Customer customer = null,
            int storeId = 0, string systemName = null)
        {
            var shippingProviders = await LoadActivePluginsAsync(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, customer, storeId);

            //filter by passed system name
            if (!string.IsNullOrEmpty(systemName))
            {
                shippingProviders = shippingProviders
                    .Where(provider => provider.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            return shippingProviders;
        }

        /// <summary>
        /// Check whether the passed shipping provider is active
        /// </summary>
        /// <param name="shippingProvider">Shipping provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IShippingRateComputationMethod shippingProvider)
        {
            return IsPluginActive(shippingProvider, _shippingSettings.ActiveShippingRateComputationMethodSystemNames);
        }

        /// <summary>
        /// Check whether the shipping provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of shipping provider to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
        {
            var shippingProvider = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
            return IsPluginActive(shippingProvider);
        }

        #endregion
    }
}