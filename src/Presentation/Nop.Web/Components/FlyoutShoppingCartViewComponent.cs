using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class FlyoutShoppingCartViewComponent : NopViewComponent
{
    protected readonly IPermissionService _permissionService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    public FlyoutShoppingCartViewComponent(IPermissionService permissionService,
        IShoppingCartModelFactory shoppingCartModelFactory,
        ShoppingCartSettings shoppingCartSettings)
    {
        _permissionService = permissionService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartSettings = shoppingCartSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!_shoppingCartSettings.MiniShoppingCartEnabled)
            return Content("");

        if (!await _permissionService.AuthorizeAsync(StandardPermission.PublicStore.ENABLE_SHOPPING_CART))
            return Content("");

        var model = await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();
        return View(model);
    }
}