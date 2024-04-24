using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Events;
using Nop.Services.Messages;

namespace Nop.Services.Customers;

/// <summary>
/// Represents a customer event consumer
/// </summary>
public class CustomerEventConsumer : IConsumer<CustomerChangeWorkingLanguageEvent>
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public CustomerEventConsumer(ICustomerService customerService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        IStoreContext storeContext)
    {
        _customerService = customerService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods
    /// <summary>
    /// Handle working language changed event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerChangeWorkingLanguageEvent eventMessage)
    {
        if (eventMessage.Customer is not Customer customer)
            return;

        if (await _customerService.IsGuestAsync(customer))
            return;

        var store = await _storeContext.GetCurrentStoreAsync();
        var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
        if (subscription != null && subscription.LanguageId != customer.LanguageId)
        {
            subscription.LanguageId = customer.LanguageId ?? 0;
            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
        }
    }

    #endregion
}
