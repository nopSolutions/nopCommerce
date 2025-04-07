using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Misc.Omnisend.Infrastructure;

/// <summary>
/// Represents plugin event consumer
/// </summary>
internal class EventConsumer(IGenericAttributeService genericAttributeService,
        OmnisendEventsService omnisendEventsService,
        OmnisendService omnisendService,
        OmnisendSettings settings) 
    : IConsumer<CustomerLoggedinEvent>,
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
    IConsumer<OrderAuthorizedEvent>,
    IConsumer<OrderPaidEvent>,
    IConsumer<OrderPlacedEvent>,
    IConsumer<OrderRefundedEvent>,
    IConsumer<OrderStatusChangedEvent>,
    IConsumer<OrderVoidedEvent>,
    IConsumer<PageRenderingEvent>
{
    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerLoggedinEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        if (string.IsNullOrEmpty(settings.IdentifyContactScript))
            return;

        var script = settings.IdentifyContactScript.Replace(OmnisendDefaults.Email, eventMessage.Customer.Email);

        await genericAttributeService.SaveAttributeAsync(eventMessage.Customer,
            OmnisendDefaults.IdentifyContactAttribute, script);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.UpdateContactAsync(eventMessage.Customer);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailSubscribedEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.UpdateOrCreateContactAsync(eventMessage.Subscription, true);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.UpdateOrCreateContactAsync(eventMessage.Subscription);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<StockQuantityHistory> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        var history = eventMessage.Entity;

        await omnisendService.UpdateProductAsync(history.ProductId);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ProductAttributeCombination> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.UpdateProductAsync(eventMessage.Entity.ProductId);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeCombination> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.UpdateProductAsync(eventMessage.Entity.ProductId);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        var entity = eventMessage.Entity;

        if (entity.ShoppingCartType != ShoppingCartType.ShoppingCart)
            return;

        await omnisendEventsService.SendAddedProductToCartEventAsync(entity);
        //await _omnisendService.AddShoppingCartItemAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendEventsService.SendOrderPlacedEventAsync(eventMessage.Order);
        await omnisendService.PlaceOrderAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendEventsService.SendOrderPaidEventAsync(eventMessage);
        //await _omnisendService.UpdateOrderAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendEventsService.SendOrderRefundedEventAsync(eventMessage);
        //await _omnisendService.UpdateOrderAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendEventsService.SendOrderStatusChangedEventAsync(eventMessage);
        //await _omnisendService.UpdateOrderAsync(eventMessage.Order);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(PageRenderingEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendEventsService.SendStartedCheckoutEventAsync(eventMessage);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return Task.CompletedTask;

        //await _omnisendService.EditShoppingCartItemAsync(eventMessage.Entity);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.DeleteShoppingCartItemAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(OrderAuthorizedEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return Task.CompletedTask;

        //await _omnisendService.UpdateOrderAsync(eventMessage.Order);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(OrderVoidedEvent eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return Task.CompletedTask;

        //await _omnisendService.UpdateOrderAsync(eventMessage.Order);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<OrderItem> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.OrderItemAddedAsync(eventMessage.Entity);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        if (!omnisendService.IsConfigured)
            return;

        await omnisendService.CreateOrUpdateProductAsync(eventMessage.Entity);
    }

    #endregion
}