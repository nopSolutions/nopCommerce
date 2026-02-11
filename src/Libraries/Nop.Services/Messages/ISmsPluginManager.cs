using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Messages;

/// <summary>
/// Represents an send sms plugin manager
/// </summary>
public partial interface ISmsPluginManager : IPluginManager<ISmsProvider>
{
    /// <summary>
    /// Load primary sms provider
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the primary sms provider
    /// </returns>
    Task<ISmsProvider> LoadPrimaryPluginAsync(Customer customer = null, int storeId = 0);

    /// <summary>
    /// Check whether the passed sms provider is active
    /// </summary>
    /// <param name="smsProvider">Sms providers to check</param>
    /// <returns>Result</returns>
    bool IsPluginActive(ISmsProvider smsProvider);

    /// <summary>
    /// Check whether the sms provider with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of sms provider to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0);
}
