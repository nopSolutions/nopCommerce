using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Plugin.Payments.AmazonPay.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Security;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.AmazonPay.Components;

/// <summary>
/// Represents a view component to render an additional field on product and category details
/// </summary>
public class DoNotUseWithAmazonPayViewComponent : NopViewComponent
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly ICategoryService _categoryService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IPermissionService _permissionService;
    private readonly IProductService _productService;

    #endregion

    #region Ctor

    public DoNotUseWithAmazonPayViewComponent(AmazonPayApiService amazonPayApiService,
        ICategoryService categoryService,
        IGenericAttributeService genericAttributeService,
        IPermissionService permissionService,
        IProductService productService)
    {
        _amazonPayApiService = amazonPayApiService;
        _categoryService = categoryService;
        _genericAttributeService = genericAttributeService;
        _permissionService = permissionService;
        _productService = productService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the widget view component
    /// </summary>
    /// <param name="widgetZone">Widget zone</param>
    /// <param name="additionalData">Additional parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        //ensure that model is passed
        if (additionalData is not BaseNopEntityModel entityModel)
            return Content(string.Empty);

        //ensure that plugin is active and configured
        if (!await _amazonPayApiService.IsActiveAndConfiguredAsync())
            return Content(string.Empty);

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS))
            return Content(string.Empty);

        //ensure that it's a proper widget zone
        if (!widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock) &&
            !widgetZone.Equals(AdminWidgetZones.CategoryDetailsBlock))
            return Content(string.Empty);

        var model = new DoNotUseWithAmazonPayModel();

        //get entity by the model identifier
        BaseEntity entity = null;

        if (widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
            entity = await _productService.GetProductByIdAsync(entityModel.Id);

        if (widgetZone.Equals(AdminWidgetZones.CategoryDetailsBlock))
            entity = await _categoryService.GetCategoryByIdAsync(entityModel.Id);

        //try to get previously saved value
        model.DoNotUseWithAmazonPay = entity != null &&
            await _genericAttributeService.GetAttributeAsync<bool>(entity, AmazonPayDefaults.DoNotUseWithAmazonPayAttributeName);

        return View("~/Plugins/Payments.AmazonPay/Views/DoNotUseWithAmazonPay.cshtml", model);
    }

    #endregion
}