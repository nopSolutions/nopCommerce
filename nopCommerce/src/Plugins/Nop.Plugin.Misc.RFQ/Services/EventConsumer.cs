using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Data;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Models;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Misc.RFQ.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer : IConsumer<AdminMenuCreatedEvent>,
    IConsumer<GetShoppingCartItemUnitPriceEvent>,
    IConsumer<ModelPreparedEvent<BaseNopModel>>,
    IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
    IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
    IConsumer<ShoppingCartItemMovedToOrderItemEvent>
{
    #region Fields

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly IRepository<ShoppingCartItem> _shoppingCartItemsRepository;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IShortTermCacheManager _shortTermCacheManager;
    private readonly IStoreContext _storeContext;
    private readonly IWidgetPluginManager _pluginManager;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public EventConsumer(IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IRepository<ShoppingCartItem> shoppingCartItemsRepository,
        IShoppingCartService shoppingCartService,
        IShortTermCacheManager shortTermCacheManager,
        IStoreContext storeContext,
        IWidgetPluginManager pluginManager,
        IWorkContext workContext,
        RfqService rfqService,
        RfqSettings rfqSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _pluginManager = pluginManager;
        _shoppingCartItemsRepository = shoppingCartItemsRepository;
        _shoppingCartService = shoppingCartService;
        _shortTermCacheManager = shortTermCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
        _rfqService = rfqService;
        _rfqSettings = rfqSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle admin menu created event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var plugin = await _pluginManager.LoadPluginBySystemNameAsync(RfqDefaults.SystemName);
        
        //the LoadPluginBySystemNameAsync method returns only plugins that are already fully installed,
        //while the IConsumer<AdminMenuCreatedEvent> event can be called before the installation is complete
        if (plugin == null || !_pluginManager.IsPluginActive(plugin))
            return;

        var menuItemSystemName = "Current shopping carts and wishlists";

        var baseMenuItem = eventMessage.RootMenuItem.GetItemBySystemName("Sales");
        baseMenuItem.InsertAfter(menuItemSystemName, new AdminMenuItem
        {
            Visible = true,
            SystemName = RfqDefaults.QuotesAdminMenuSystemName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.Quotes"),
            Url = eventMessage.GetMenuItemUrl("RfqAdmin", "AdminQuotes"),
            IconClass = "far fa-dot-circle",
            PermissionNames = new List<string> { RfqPermissionConfigManager.ADMIN_ACCESS_RFQ }
        });
        baseMenuItem.InsertAfter(menuItemSystemName, new AdminMenuItem
        {
            Visible = true,
            SystemName = RfqDefaults.RequestsAdminMenuSystemName,
            Title = await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.RequestsQuote"),
            Url = eventMessage.GetMenuItemUrl("RfqAdmin", "AdminRequests"),
            IconClass = "far fa-dot-circle",
            PermissionNames = new List<string> { RfqPermissionConfigManager.ADMIN_ACCESS_RFQ }
        });
    }

    /// <summary>
    /// Handle get shopping cart item unit price event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(GetShoppingCartItemUnitPriceEvent eventMessage)
    {
        if (!_rfqSettings.Enabled)
            return;

        var quoteItem = await _shortTermCacheManager.GetAsync(async () => await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(eventMessage.ShoppingCartItem.Id), RfqDefaults.QuoteItemByShoppingCartItemCacheKey, eventMessage.ShoppingCartItem.Id);

        if (quoteItem == null)
            return;

        eventMessage.DiscountAmount = 0;
        eventMessage.AppliedDiscounts?.Clear();
        eventMessage.UnitPrice = quoteItem.OfferedUnitPrice;
        eventMessage.StopProcessing = true;
    }

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (!_rfqSettings.Enabled)
            return;

        if (eventMessage.Model is ShoppingCartModel model)
        {
            var routeName = _httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;

            if (routeName is not NopRouteNames.General.CART)
                return;

            //is shopping cart created by quote
            if (await model.Items.AnyAwaitAsync(async shoppingCartItemModel => (await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(shoppingCartItemModel.Id)) == null))
                return;

            var disableEdit = false;

            foreach (var shoppingCartItemModel in model.Items)
            {
                var quoteItem = await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(shoppingCartItemModel.Id);

                if (quoteItem == null)
                    return;

                shoppingCartItemModel.AllowItemEditing = false;
                shoppingCartItemModel.DisableRemoval = true;
                shoppingCartItemModel.Quantity = quoteItem.OfferedQty;

                disableEdit = true;
            }

            if (disableEdit)
            {
                model.IsEditable = false;
                model.IsReadyToCheckout = true;
            }
        }
    }

    /// <summary>
    /// Handle shopping cart item inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        if (!_rfqSettings.Enabled)
            return;

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        var quoteItems =
            await _rfqService.GetQuoteItemsByShoppingCartItemIdsAsync(cart.Select(sci => sci.Id).ToArray());

        foreach (var quoteItem in quoteItems)
        {
            await _shoppingCartService.DeleteShoppingCartItemAsync(quoteItem.ShoppingCartItemId!.Value);
            _shortTermCacheManager.Remove(RfqDefaults.QuoteItemByShoppingCartItemCacheKey.Key, quoteItem.ShoppingCartItemId);
            quoteItem.ShoppingCartItemId = null;
        }

        await _rfqService.UpdateQuoteItemsAsync(quoteItems);
    }

    /// <summary>
    /// Handle shopping cart item moved to order item event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(ShoppingCartItemMovedToOrderItemEvent eventMessage)
    {
        if (!_rfqSettings.Enabled)
            return;

        var quoteItem = await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(eventMessage.ShoppingCartItem.Id);

        if (quoteItem == null)
            return;

        var quote = await _rfqService.GetQuoteByIdAsync(quoteItem.QuoteId);
        quote.Status = QuoteStatus.OrderCreated;
        quote.OrderId = eventMessage.OrderItem.OrderId;
        await _rfqService.UpdateQuoteAsync(quote);

        var items = await _rfqService.GetQuoteItemsAsync(quote.Id);

        foreach (var item in items)
        {
            _shortTermCacheManager.Remove(RfqDefaults.QuoteItemByShoppingCartItemCacheKey.Key, quoteItem.ShoppingCartItemId);
            item.ShoppingCartItemId = null;
        }

        await _rfqService.UpdateQuoteItemsAsync(items);
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
    {
        if (!_rfqSettings.Enabled)
            return;

        var quoteItem = await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(eventMessage.Entity.Id);
        
        if (quoteItem == null || eventMessage.Entity.Quantity == quoteItem.OfferedQty)
            return;

        eventMessage.Entity.Quantity = quoteItem.OfferedQty;

        await _shoppingCartItemsRepository.UpdateAsync(eventMessage.Entity);
    }

    #endregion
}