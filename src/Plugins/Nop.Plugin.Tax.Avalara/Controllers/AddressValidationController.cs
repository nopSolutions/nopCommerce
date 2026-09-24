using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Tax;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Tax.Avalara.Controllers;

[AutoValidateAntiforgeryToken]
public class AddressValidationController : BaseController
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IWorkContext _workContext;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public AddressValidationController(ICustomerService customerService,
        IWorkContext workContext,
        TaxSettings taxSettings)
    {
        _customerService = customerService;
        _workContext = workContext;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> UseValidatedAddress(int addressId)
    {
        //try to get an address by the passed identifier
        var customer = await _workContext.GetCurrentCustomerAsync();
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);
        if (address != null)
        {
            //and update appropriate customer address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress)
                customer.BillingAddressId = address.Id;
            if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress)
                customer.ShippingAddressId = address.Id;
            await _customerService.UpdateCustomerAsync(customer);
        }

        //nothing to return
        return Content(string.Empty);
    }

    #endregion
}