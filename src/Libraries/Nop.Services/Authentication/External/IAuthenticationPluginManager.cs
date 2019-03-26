using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Authentication.External
{
    /// <summary>
    /// Represents an authentication plugin manager
    /// </summary>
    public partial interface IAuthenticationPluginManager : IPluginManager<IExternalAuthenticationMethod>
    {
        /// <summary>
        /// Load active authentication methods
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of active authentication methods</returns>
        IList<IExternalAuthenticationMethod> LoadActivePlugins(Customer customer = null, int storeId = 0);

        /// <summary>
        /// Check whether the passed authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Authentication method to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IExternalAuthenticationMethod authenticationMethod);

        /// <summary>
        /// Check whether the authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of authentication method to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        bool IsPluginActive(string systemName, Customer customer = null, int storeId = 0);
    }
}