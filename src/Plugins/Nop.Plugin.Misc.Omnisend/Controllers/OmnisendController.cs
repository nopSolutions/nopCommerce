using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.Omnisend.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.Omnisend.Controllers;

public class OmnisendController(ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IWebHelper webHelper,
        IWorkContext workContext,
        OmnisendService omnisendService) 
    : BasePluginController
{
    #region Methods

    public async Task<IActionResult> AbandonedCheckout(string cartId)
    {
        var customer = await workContext.GetCurrentCustomerAsync();
        if (await customerService.IsGuestAsync(customer))
            return RedirectToRoute("Login", new { ReturnUrl = webHelper.GetRawUrl(Request) });

        var customerEmail = await genericAttributeService.GetAttributeAsync<string>(customer, OmnisendDefaults.CustomerEmailAttribute);
        if (!string.IsNullOrEmpty(customerEmail) && !customerEmail.Equals(customer.Email, StringComparison.InvariantCultureIgnoreCase))
            return RedirectToRoute("Login", new { ReturnUrl = webHelper.GetRawUrl(Request) });

        await omnisendService.RestoreShoppingCartAsync(cartId);

        return RedirectToRoute("ShoppingCart");
    }

    #endregion
}