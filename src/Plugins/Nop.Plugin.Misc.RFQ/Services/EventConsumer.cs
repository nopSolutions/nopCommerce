using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
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
    IConsumer<ShoppingCartItemMovedToOrderItemEvent>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;

    public EventConsumer(IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IShoppingCartService shoppingCartService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IWorkContext workContext,
        RfqService rfqService)
    {
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _shoppingCartService = shoppingCartService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _workContext = workContext;
        _rfqService = rfqService;
    }

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        var visible = !await _permissionService.AuthorizeAsync(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ);

        var menuItemSystemName = "Current shopping carts and wishlists";

        var baseMenuItem = eventMessage.RootMenuItem.GetItemBySystemName("Sales");
        baseMenuItem.InsertAfter(menuItemSystemName,
            new AdminMenuItem
            {
                Visible = visible,
                SystemName = RfqDefaults.RequestsAdminMenuSystemName,
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.RequestsQuote"),
                Url = eventMessage.GetMenuItemUrl("RfqAdmin", "AdminRequests"),
                IconClass = "far fa-dot-circle"
            });
        baseMenuItem.InsertAfter(menuItemSystemName,
            new AdminMenuItem
            {
                Visible = visible,
                SystemName = RfqDefaults.QuotesAdminMenuSystemName,
                Title = await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.Quotes"),
                Url = eventMessage.GetMenuItemUrl("RfqAdmin", "AdminQuotes"),
                IconClass = "far fa-dot-circle"
            });
    }

    public async Task HandleEventAsync(GetShoppingCartItemUnitPriceEvent eventMessage)
    {
        var quoteItem = await _staticCacheManager.GetAsync(
            _staticCacheManager.PrepareKey(RfqDefaults.QuoteItemByShoppingCartItemCacheKey,
                eventMessage.ShoppingCartItem.Id),
            async () => await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(eventMessage.ShoppingCartItem.Id));

        if (quoteItem == null)
            return;

        eventMessage.DiscountAmount = 0;
        eventMessage.AppliedDiscounts?.Clear();
        eventMessage.UnitPrice = quoteItem.OfferedUnitPrice;
        eventMessage.StopProcessing = true;
    }

    public async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is ShoppingCartModel model)
        {
            var routeName = _httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;

            if (routeName is not "ShoppingCart")
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
                shoppingCartItemModel.AllowedQuantities ??= new List<SelectListItem>();
                shoppingCartItemModel.AllowedQuantities.Clear();
                shoppingCartItemModel.AllowedQuantities.Add(new SelectListItem(quoteItem.OfferedQty.ToString(),
                    quoteItem.OfferedQty.ToString(), true));

                disableEdit = true;
            }

            if (disableEdit)
            {
                model.IsEditable = false;
                model.IsReadyToCheckout = true;
            }
        }
    }

    public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        var quoteItems =
            await _rfqService.GetQuoteItemsByShoppingCartItemIdsAsync(cart.Select(sci => sci.Id).ToArray());

        foreach (var quoteItem in quoteItems)
        {
            await _shoppingCartService.DeleteShoppingCartItemAsync(quoteItem.ShoppingCartItemId!.Value);
            await _staticCacheManager.RemoveAsync(RfqDefaults.QuoteItemByShoppingCartItemCacheKey, quoteItem.ShoppingCartItemId);
            quoteItem.ShoppingCartItemId = null;
        }

        await _rfqService.UpdateQuoteItemsAsync(quoteItems);
    }

    public async Task HandleEventAsync(ShoppingCartItemMovedToOrderItemEvent eventMessage)
    {
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
            await _staticCacheManager.RemoveAsync(RfqDefaults.QuoteItemByShoppingCartItemCacheKey, quoteItem.ShoppingCartItemId);
            item.ShoppingCartItemId = null;
        }

        await _rfqService.UpdateQuoteItemsAsync(items);
    }
}