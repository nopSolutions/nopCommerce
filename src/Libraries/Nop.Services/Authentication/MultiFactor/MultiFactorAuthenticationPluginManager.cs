using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Authentication.MultiFactor
{
    /// <summary>
    /// Represents an multi-factor authentication plugin manager implementation
    /// </summary>
    public partial class MultiFactorAuthenticationPluginManager : PluginManager<IMultiFactorAuthenticationMethod>, IMultiFactorAuthenticationPluginManager
    {
        #region Fields

        private readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

        #endregion

        #region Ctor

        public MultiFactorAuthenticationPluginManager(MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
            ICustomerService customerService,
            IPluginService pluginService) : base(customerService, pluginService)
        {
            _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check is active multi-factor authentication methods
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>True - if active multi-factor authentication methods</returns>
        public virtual bool HasActivePlugins(Customer customer = null, int storeId = 0)
        {
            return LoadActivePlugins(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, customer, storeId).Any();
        }

        /// <summary>
        /// Load active multi-factor authentication methods
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>List of active multi-factor authentication methods</returns>
        public virtual IList<IMultiFactorAuthenticationMethod> LoadActivePlugins(Customer customer = null, int storeId = 0)
        {
            return LoadActivePlugins(_multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames, customer, storeId);
        }

        /// <summary>
        /// Check whether the passed multi-factor authentication method is active
        /// </summary>
        /// <param name="authenticationMethod">Authentication method to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IMultiFactorAuthenticationMethod authenticationMethod)
        {
            return IsPluginActive(authenticationMethod, _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames);
        }

        /// <summary>
        /// Check whether the multi-factor authentication method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of authentication method to check</param>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(string systemName, Customer customer = null, int storeId = 0)
        {
            var authenticationMethod = LoadPluginBySystemName(systemName, customer, storeId);
            return IsPluginActive(authenticationMethod);
        }

        #endregion

    }
}
