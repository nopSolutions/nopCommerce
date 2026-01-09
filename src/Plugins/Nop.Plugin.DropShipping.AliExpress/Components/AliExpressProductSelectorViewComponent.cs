using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.DropShipping.AliExpress.Models;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.DropShipping.AliExpress.Components;

/// <summary>
/// View component for AliExpress product selector
/// </summary>
[ViewComponent(Name = "AliExpressProductSelector")]
public class AliExpressProductSelectorViewComponent : NopViewComponent
{
    private readonly IAliExpressProductMappingService _mappingService;
    private readonly IPermissionService _permissionService;

    public AliExpressProductSelectorViewComponent(
        IAliExpressProductMappingService mappingService,
        IPermissionService permissionService)
    {
        _mappingService = mappingService;
        _permissionService = permissionService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        // Check permission
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE))
            return Content(string.Empty);

        // Ensure additionalData is ProductModel
        if (additionalData is not ProductModel productModel)
            return Content(string.Empty);

        var model = new ProductSelectorModel
        {
            ProductId = productModel.Id
        };

        // Load existing mapping if exists
        if (productModel.Id > 0)
        {
            var mapping = await _mappingService.GetMappingByProductIdAsync(productModel.Id);
            if (mapping != null)
            {
                model.AliExpressProductId = mapping.AliExpressProductId.ToString();
            }
        }

        return View("~/Plugins/DropShipping.AliExpress/Views/ProductSelector.cshtml", model);
    }
}
