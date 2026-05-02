using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Events;

namespace Nop.Services.Orders;

/// <summary>
/// Represents AppStarted event consumer
/// </summary>
public partial class AppStartedConsumer : IConsumer<AppStartedEvent>
{
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;

    #endregion

    #region Ctor

    public AppStartedConsumer(IGenericAttributeService genericAttributeService)
    {
        _genericAttributeService = genericAttributeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(AppStartedEvent eventMessage)
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //clear payment info requests
        await _genericAttributeService.DeleteAttributesAsync<Customer>(NopCustomerDefaults.ProcessPaymentRequestAttribute);
    }

    #endregion
}
