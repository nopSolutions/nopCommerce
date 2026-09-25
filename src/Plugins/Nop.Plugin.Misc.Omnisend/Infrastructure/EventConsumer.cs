using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.Omnisend.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
internal class EventConsumer : IConsumer<CustomerLoggedinEvent>,
    IConsumer<CustomerRegisteredEvent>,
    IConsumer<EmailSubscribedEvent>,
    IConsumer<EmailUnsubscribedEvent>,
    IConsumer<EntityDeletedEvent<ProductAttributeCombination>>,
    IConsumer<EntityDeletedEvent<ShoppingCartItem>>,
    IConsumer<EntityUpdatedEvent<Product>>,
    IConsumer<EntityInsertedEvent<ProductAttributeCombination>>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityInsertedEvent<StockQuantityHistory>>,
    IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
    IConsumer<EntityInsertedEvent<OrderItem>>,
    IConsumer<OrderPaidEvent>,
    IConsumer<OrderPlacedEvent>,
    IConsumer<OrderRefundedEvent>,
    IConsumer<OrderStatusChangedEvent>,
    IConsumer<PageRenderingEvent>
{
    #region Fields

    private readonly IGenericAttributeService _genericAttributeService;
    private readonly OmnisendEventsService _omnisendEventsService;
    private readonly OmnisendService _omnisendService;
    private readonly OmnisendSettings _settings;

    #endregion

    #region Ctor

    public EventConsumer(IGenericAttributeService genericAttributeService,
        OmnisendEventsService omnisendEventsService,
        OmnisendService omnisendService,
        OmnisendSettings settings)
    {
        _genericAttributeService = genericAttributeService;
        _omnisendEventsService = omnisendEventsService;
        _omnisendService = omnisendService;
        _settings = settings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerLoggedinEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        if (string.IsNullOrEmpty(_settings.IdentifyContactScript))
            return;

        var script = _settings.IdentifyContactScript.Replace(OmnisendDefaults.Email, eventMessage.Customer.Email);

        await _genericAttributeService.SaveAttributeAsync(eventMessage.Customer,
            OmnisendDefaults.IdentifyContactAttribute, script);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendService.UpdateContactAsync(eventMessage.Customer);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailSubscribedEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendService.UpdateOrCreateContactAsync(eventMessage.Subscription, true);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendService.UpdateOrCreateContactAsync(eventMessage.Subscription);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<StockQuantityHistory> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        var history = eventMessage.Entity;

        await _omnisendService.UpdateProductAsync(history.ProductId);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ProductAttributeCombination> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendService.UpdateProductAsync(eventMessage.Entity.ProductId);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeCombination> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendService.UpdateProductAsync(eventMessage.Entity.ProductId);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        var entity = eventMessage.Entity;

        if (entity.ShoppingCartType != ShoppingCartType.ShoppingCart)
            return;

        await _omnisendEventsService.SendAddedProductToCartEventAsync(entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendEventsService.SendOrderPlacedEventAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendEventsService.SendOrderPaidEventAsync(eventMessage);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendEventsService.SendOrderRefundedEventAsync(eventMessage);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendEventsService.SendOrderStatusChangedEventAsync(eventMessage);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(PageRenderingEvent eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        var routeName = eventMessage.GetRouteName();
        if (!routeName.Equals(NopRouteNames.Standard.CHECKOUT_ONE_PAGE, StringComparison.InvariantCultureIgnoreCase) &&
            !routeName.Equals(NopRouteNames.Standard.CHECKOUT_BILLING_ADDRESS, StringComparison.InvariantCultureIgnoreCase))
            return;

        await _omnisendEventsService.SendStartedCheckoutEventAsync();
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendEventsService.SendStartedCheckoutEventAsync();
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendEventsService.SendStartedCheckoutEventAsync();
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<OrderItem> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendService.OrderItemAddedAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        if (!_omnisendService.IsConfigured)
            return;

        await _omnisendService.CreateOrUpdateProductAsync(eventMessage.Entity);
    }

    #endregion
}