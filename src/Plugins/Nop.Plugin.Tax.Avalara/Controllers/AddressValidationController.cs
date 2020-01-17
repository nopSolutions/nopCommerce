using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Tax.Avalara.Controllers
{
    public class AddressValidationController : BaseController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public AddressValidationController(IAddressService addressService,
            ICustomerService customerService,
            IWorkContext workContext,
            TaxSettings taxSettings)
        {
            _addressService = addressService;
            _customerService = customerService;
            _workContext = workContext;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Methods

        [HttpPost]
        public IActionResult UseValidatedAddress(int addressId, bool isNewAddress)
        {
            //try to get an address by the passed identifier
            var address = _addressService.GetAddressById(addressId);
            if (address != null)
            {
                //add address to customer collection if it's a new
                if (isNewAddress)
                    _workContext.CurrentCustomer.Addresses.Add(address);

                //and update appropriate customer address
                if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress)
                    _workContext.CurrentCustomer.BillingAddress = address;
                if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress)
                    _workContext.CurrentCustomer.ShippingAddress = address;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
            }

            //nothing to return
            return Content(string.Empty);
        }

        #endregion
    }
}