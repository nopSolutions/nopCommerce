using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Widgets.What3words.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer :
    IConsumer<EntityDeletedEvent<Address>>,
    IConsumer<EntityInsertedEvent<CustomerAddressMapping>>,
    IConsumer<ModelReceivedEvent<BaseNopModel>>,
    IConsumer<OrderPlacedEvent>
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;
    protected readonly What3wordsSettings _what3WordsSettings;

    #endregion

    #region Ctor

    public EventConsumer(IAddressService addressService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext,
        What3wordsSettings what3WordsSettings)
    {
        _addressService = addressService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _widgetPluginManager = widgetPluginManager;
        _workContext = workContext;
        _what3WordsSettings = what3WordsSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle entity deleted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<Address> eventMessage)
    {
        if (eventMessage.Entity is not Address address)
            return;

        await _genericAttributeService.SaveAttributeAsync<string>(address, What3wordsDefaults.AddressValueAttribute, null);
    }

    /// <summary>
    /// Handle entity inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<CustomerAddressMapping> eventMessage)
    {
        if (eventMessage.Entity is not CustomerAddressMapping mapping)
            return;

        //move previously cached value to the address
        if (_httpContextAccessor.HttpContext.Items.TryGetValue(What3wordsDefaults.AddressValueAttribute, out var addressValue))
        {
            var address = await _addressService.GetAddressByIdAsync(mapping.AddressId);
            if (address is not null)
                await _genericAttributeService.SaveAttributeAsync(address, What3wordsDefaults.AddressValueAttribute, addressValue);
        }
    }

    /// <summary>
    /// Handle model received event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not CheckoutBillingAddressModel &&
            eventMessage.Model is not CheckoutShippingAddressModel)
            return;

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _widgetPluginManager.IsPluginActiveAsync(What3wordsDefaults.SystemName, customer))
            return;

        if (!_what3WordsSettings.Enabled)
            return;

        //cache the value within the request, we save it to the address later
        var form = await _httpContextAccessor.HttpContext.Request.ReadFormAsync();
        if (form.TryGetValue(What3wordsDefaults.ComponentName, out var addressValue) && !StringValues.IsNullOrEmpty(addressValue))
            _httpContextAccessor.HttpContext.Items[What3wordsDefaults.AddressValueAttribute] = addressValue.ToString().TrimStart('/');
    }

    /// <summary>
    /// Handle order placed event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        if (eventMessage.Order is null)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
        if (!await _widgetPluginManager.IsPluginActiveAsync(What3wordsDefaults.SystemName, customer))
            return;

        if (!_what3WordsSettings.Enabled)
            return;

        async Task copyAddressValueAsync(int? customerAddressId, int? orderAddressId)
        {
            var customerAddress = await _addressService.GetAddressByIdAsync(customerAddressId ?? 0);
            var addressValue = customerAddress is not null
                ? await _genericAttributeService.GetAttributeAsync<string>(customerAddress, What3wordsDefaults.AddressValueAttribute)
                : null;
            if (!string.IsNullOrEmpty(addressValue))
            {
                var orderAddress = await _addressService.GetAddressByIdAsync(orderAddressId ?? 0);
                if (orderAddress is not null)
                    await _genericAttributeService.SaveAttributeAsync(orderAddress, What3wordsDefaults.AddressValueAttribute, addressValue);
            }
        }

        //copy values from customer addresses to order addresses for next use
        await copyAddressValueAsync(customer.BillingAddressId, eventMessage.Order.BillingAddressId);
        await copyAddressValueAsync(customer.ShippingAddressId, eventMessage.Order.ShippingAddressId);
    }

    #endregion
}