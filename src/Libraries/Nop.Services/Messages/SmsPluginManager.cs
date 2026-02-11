using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;

namespace Nop.Services.Messages;

/// <summary>
/// Represents an send sms plugin manager implementation
/// </summary>
public partial class SmsPluginManager : PluginManager<ISmsProvider>, ISmsPluginManager
{
    #region Fields

    protected readonly OtpSettings _otpSettings;

    #endregion

    #region Ctor
    public SmsPluginManager(ICustomerService customerService, 
        IPluginService pluginService,
        OtpSettings otpSettings) : base(customerService, pluginService)
    {
        _otpSettings = otpSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Load primary sms provider
    /// </summary>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the primary sms provider
    /// </returns>
    public virtual async Task<ISmsProvider> LoadPrimaryPluginAsync(Customer customer = null, int storeId = 0)
    {
        return await LoadPrimaryPluginAsync(_otpSettings.ActiveSmsProviderSystemName, customer, storeId);
    }

    /// <summary>
    /// Check whether the passed sms provider is active
    /// </summary>
    /// <param name="smsProvider">Sms provider to check</param>
    /// <returns>Result</returns>
    public virtual bool IsPluginActive(ISmsProvider smsProvider)
    {
        return IsPluginActive(smsProvider, [_otpSettings.ActiveSmsProviderSystemName]);
    }

    /// <summary>
    /// Check whether the sms provider with the passed system name is active
    /// </summary>
    /// <param name="systemName">System name of send sms method to check</param>
    /// <param name="customer">Filter by customer; pass null to load all plugins</param>
    /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
        var sendSmsMethod = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
        return IsPluginActive(sendSmsMethod);
    }

    #endregion
}
