using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Shipping.Pickup
{
    /// <summary>
    /// Represents a pickup point plugin manager
    /// </summary>
    public partial interface IPickupPluginManager : IPluginManager<IPickupPointProvider>
    {
        /// <summary>
        /// Load active pickup point providers
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="systemName">Filter by pickup point provider system name; pass null to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of active pickup point providers
        /// </returns>
        Task<IList<IPickupPointProvider>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0, string systemName = null);

        /// <summary>
        /// Check whether the passed pickup point provider is active
        /// </summary>
        /// <param name="pickupPointProvider">Pickup point provider to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IPickupPointProvider pickupPointProvider);

        /// <summary>
        /// Check whether the pickup point provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of pickup point provider to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0);
    }
}