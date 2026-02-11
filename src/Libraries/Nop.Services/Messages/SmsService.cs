using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Messages;

public partial class SmsService : ISmsService
{
    #region Fields

    protected readonly ISmsPluginManager _smsPluginManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SmsService(ISmsPluginManager smsPluginManager, 
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _smsPluginManager = smsPluginManager;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sends an SMS message with the specified text content asynchronously
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <param name="text">The text content of the SMS message to send</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the boolean result of sending SMS message
    /// </returns>
    public virtual async Task<bool> SendSmsAsync(string phoneNumber, string text)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();

        var smsProvider = await _smsPluginManager.LoadPrimaryPluginAsync(customer, store.Id);
        
        return await smsProvider.SendSmsAsync(phoneNumber, text);
    }

    #endregion
}
