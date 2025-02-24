using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Misc.RFQ.Components;

/// <summary>
/// Represents the view component to display an item in the customer menu
/// </summary>
public class CustomerRfqMenuComponent : NopViewComponent
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public CustomerRfqMenuComponent(ICustomerService customerService, 
        IWorkContext workContext,
        RfqSettings rfqSettings)
    {
        _customerService = customerService;
        _workContext = workContext;
        _rfqSettings = rfqSettings;
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
        if (additionalData is not CustomerNavigationModel model)
            return Content(string.Empty);

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Content(string.Empty);

        if (!_rfqSettings.Enabled)
            return Content(string.Empty);

        return View("~/Plugins/Misc.RFQ/Views/Components/CustomerRfqMenu.cshtml", model);
    }

    #endregion
}