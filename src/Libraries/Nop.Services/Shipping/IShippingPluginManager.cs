using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Represents a shipping plugin manager
    /// </summary>
    public partial interface IShippingPluginManager : IPluginManager<IShippingRateComputationMethod>
    {
        /// <summary>
        /// Load active shipping providers
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by shipping provider system name; pass null to load all plugins</param>
        /// <returns>List of active shipping providers</returns>
        IList<IShippingRateComputationMethod> LoadActivePlugins(Customer customer = null, int storeId = 0, string systemName = null);

        /// <summary>
        /// Check whether the passed shipping provider is active
        /// </summary>
        /// <param name="shippingProvider">Shipping provider to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IShippingRateComputationMethod shippingProvider);

        /// <summary>
        /// Check whether the shipping provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of shipping provider to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        bool IsPluginActive(string systemName, Customer customer = null, int storeId = 0);
    }
}