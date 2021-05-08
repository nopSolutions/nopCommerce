using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a tax plugin manager
    /// </summary>
    public partial interface ITaxPluginManager : IPluginManager<ITaxProvider>
    {
        /// <summary>
        /// Load primary active tax provider
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax provider
        /// </returns>
        Task<ITaxProvider> LoadPrimaryPluginAsync(Customer customer = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed tax provider is active
        /// </summary>
        /// <param name="taxProvider">Tax provider to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(ITaxProvider taxProvider);

        /// <summary>
        /// Check whether the tax provider with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of tax provider to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0);
    }
}