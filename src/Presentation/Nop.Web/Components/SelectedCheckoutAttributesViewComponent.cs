using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class SelectedCheckoutAttributesViewComponent : NopViewComponent
{
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;

    public SelectedCheckoutAttributesViewComponent(IShoppingCartModelFactory shoppingCartModelFactory)
    {
        _shoppingCartModelFactory = shoppingCartModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var attributes = await _shoppingCartModelFactory.FormatSelectedCheckoutAttributesAsync();
        return View(null, attributes);
    }
}