using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Components;

public partial class OrderSummaryViewComponent : NopViewComponent
{
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    public OrderSummaryViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
    /// <param name="overriddenModel">Overridden model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(bool? prepareAndDisplayOrderReviewData, ShoppingCartModel overriddenModel)
    {
        //use already prepared (shared) model
        if (overriddenModel != null)
            return await ViewAsync(overriddenModel);

        //if not passed, then create a new model
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);

        var model = new ShoppingCartModel();
        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart,
            isEditable: false,
            prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
        return await ViewAsync(model);
    }
}