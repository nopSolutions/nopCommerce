using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Checkout;

namespace Nop.Web.Components;

public partial class CheckoutProgressViewComponent : NopViewComponent
{
    protected readonly ICheckoutModelFactory _checkoutModelFactory;

    public CheckoutProgressViewComponent(ICheckoutModelFactory checkoutModelFactory)
    {
        _checkoutModelFactory = checkoutModelFactory;
    }

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="step">Checkout progress step</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(CheckoutProgressStep step)
    {
        var model = await _checkoutModelFactory.PrepareCheckoutProgressModelAsync(step);
        return await ViewAsync(model);
    }
}