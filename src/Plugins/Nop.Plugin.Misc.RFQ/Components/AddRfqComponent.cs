using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.RFQ.Components;

public class AddRfqComponent : NopViewComponent
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;

    public AddRfqComponent(IShoppingCartService shoppingCartService,
        IWorkContext workContext,
        RfqService rfqService)
    {
        _shoppingCartService = shoppingCartService;
        _workContext = workContext;
        _rfqService = rfqService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync());

        //is shopping cart created by quote
        if (await cart.AnyAwaitAsync(async shoppingCartItemModel => (await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(shoppingCartItemModel.Id)) != null))
            return Content(string.Empty);

        return View("~/Plugins/Misc.RFQ/Views/Components/AddRfq.cshtml");
    }
}
