using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Authentication.External;

/// <summary>
/// Represents an authentication plugin manager implementation
/// </summary>
public partial class AuthenticationPluginManager : PluginManager<IExternalAuthenticationMethod>, IAuthenticationPluginManager
{
    #region Fields

    protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;

    #endregion

    #region Ctor

    public AuthenticationPluginManager(ExternalAuthenticationSettings externalAuthenticationSettings,
        ICustomerService customerService,
        IPluginService pluginService) : base(customerService, pluginService)
    {
        _externalAuthenticationSettings = externalAuthenticationSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load active authentication methods
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of active authentication methods
    /// </returns>
    public virtual async Task<IList<IExternalAuthenticationMethod>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0)
    {
        return await LoadActivePluginsAsync(_externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames, customer, storeId);
    }

    /// <summary>
    /// Check whether the passed authentication method is active
    /// </summary>
    /// <param name="authenticationMethod">Authentication method to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(IExternalAuthenticationMethod authenticationMethod)
    {
        return IsPluginActive(authenticationMethod, _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames);
    }

    /// <summary>
    /// Check whether the authentication method with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of authentication method to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
        var authenticationMethod = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
        return IsPluginActive(authenticationMethod);
    }

    #endregion
}