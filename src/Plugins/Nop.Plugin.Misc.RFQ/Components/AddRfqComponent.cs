using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.RFQ.Components;

/// <summary>
/// Represents the view component to display additional buttons in the shopping cart
/// </summary>
public class AddRfqComponent : NopViewComponent
{
    #region Fields

    private readonly IProductService _productService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;

    #endregion

    #region Ctor

    public AddRfqComponent(IProductService productService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        RfqService rfqService)
    {
        _productService = productService;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
        _rfqService = rfqService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        var products = await _productService.GetProductsByIdsAsync(cart.Select(i => i.ProductId).ToArray());

        //"enter your price" or rental products in the cart, so we should not show the "Request a quote" button
        if (products.Any(product => product.CustomerEntersPrice || product.IsRental))
            return Content(string.Empty);

        //is shopping cart created by quote
        if (await cart.AnyAwaitAsync(async shoppingCartItemModel => (await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(shoppingCartItemModel.Id)) != null))
            return View("~/Plugins/Misc.RFQ/Views/Components/ClearRfq.cshtml");

        return View("~/Plugins/Misc.RFQ/Views/Components/AddRfq.cshtml");
    }

    #endregion
}