using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Authentication.MultiFactor
{
    /// <summary>
    /// Represents an multi-factor authentication plugin manager
    /// </summary>
    public partial interface IMultiFactorAuthenticationPluginManager : IPluginManager<IMultiFactorAuthenticationMethod>
    {
        // <summary>
        /// Check is active multi-factor authentication methods
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>True - if active multi-factor authentication methods</returns>
        bool HasActivePlugins(Customer customer = null, int storeId = 0);

        /// <summary>
        /// Load active multi-factor authentication methods
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of active multi-factor authentication methods</returns>
        IList<IMultiFactorAuthenticationMethod> LoadActivePlugins(Customer customer = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed multi-factor authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Multi-factor authentication method to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IMultiFactorAuthenticationMethod authenticationMethod);

        /// <summary>
        /// Check whether the multi-factor authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of multi-factor authentication method to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        bool IsPluginActive(string systemName, Customer customer = null, int storeId = 0);
    }
}
