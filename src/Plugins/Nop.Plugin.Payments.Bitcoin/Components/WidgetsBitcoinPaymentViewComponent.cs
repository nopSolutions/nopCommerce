using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Bitcoin.Models;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Bitcoin.Components;

/// <summary>
/// Represents view component to embed test widget on pages
/// </summary>
public class WidgetsBitcoinPaymentViewComponent : NopViewComponent
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public WidgetsBitcoinPaymentViewComponent(ICustomerService customerService, IWorkContext workContext)
    {
        _customerService = customerService;
        _workContext = workContext;
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
        return View("~/Plugins/Payments.Bitcoin/Views/TestHtml.cshtml", new BitcoinPaymentInfoModel
        {
            SomeText = "Hello!!!!!!!"
        });
    }

    #endregion
}