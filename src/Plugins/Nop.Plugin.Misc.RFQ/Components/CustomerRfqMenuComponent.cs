using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Misc.RFQ.Components;

public class CustomerRfqMenuComponent : NopViewComponent
{
    private readonly ICustomerService _customerService;
    private readonly IWorkContext _workContext;

    public CustomerRfqMenuComponent(ICustomerService customerService, IWorkContext workContext)
    {
        _customerService = customerService;
        _workContext = workContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(string _, object additionalData)
    {
        if (additionalData is not CustomerNavigationModel model)
            return Content(string.Empty);

        if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()))
            return Content(string.Empty);

        return View("~/Plugins/Misc.RFQ/Views/Components/CustomerRfqMenu.cshtml", model);
    }
}
