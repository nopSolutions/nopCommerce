using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.Avalara.Models.Checkout;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Tax.Avalara.Components
{
    /// <summary>
    /// Represents a view component to validate entered address and display a confirmation dialog on the checkout page
    /// </summary>
    [ViewComponent(Name = AvalaraTaxDefaults.ADDRESS_VALIDATION_VIEW_COMPONENT_NAME)]
    public class AddressValidationViewComponent : NopViewComponent
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public AddressValidationViewComponent(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            IAddressService addressService,
            ICountryService countryService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            ITaxPluginManager taxPluginManager,
            IWorkContext workContext,
            TaxSettings taxSettings)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _addressService = addressService;
            _countryService = countryService;
            _customerService = customerService;
            _localizationService = localizationService;
            _stateProvinceService = stateProvinceService;
            _taxPluginManager = taxPluginManager;
            _workContext = workContext;
            _taxSettings = taxSettings;
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
            //ensure that Avalara tax provider is active
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
                return Content(string.Empty);

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(PublicWidgetZones.CheckoutConfirmTop) && !widgetZone.Equals(PublicWidgetZones.OpCheckoutConfirmTop))
                return Content(string.Empty);

            //ensure thet address validation is enabled
            if (!_avalaraTaxSettings.ValidateAddress)
                return Content(string.Empty);

            //validate entered by customer addresses only
            var addressId = _taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress
                ? customer.BillingAddressId
                : _taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress
                ? customer.ShippingAddressId
                : null;

            var address = await _addressService.GetAddressByIdAsync(addressId ?? 0);
            if (address == null)
                return Content(string.Empty);

            //validate address
            var validationResult = await _avalaraTaxManager.ValidateAddressAsync(address);

            //whether there are errors in validation result
            var errorDetails = validationResult?.messages?
                .Where(message => message.severity.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                .Select(message => message.details)
                ?? new List<string>();
            if (errorDetails.Any())
            {
                //display error message to customer
                return View("~/Plugins/Tax.Avalara/Views/Checkout/AddressValidation.cshtml", new AddressValidationModel
                {
                    Message = string.Format(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.AddressValidation.Error"),
                        WebUtility.HtmlEncode(string.Join("; ", errorDetails))),
                    IsError = true
                });
            }

            //if there are no errors and no validated addresses, nothing to display
            if (!validationResult?.validatedAddresses?.Any() ?? true)
                return Content(string.Empty);

            //get validated address info
            var validatedAddressInfo = validationResult.validatedAddresses.FirstOrDefault();

            //create new address as a copy of address to validate and with details of the validated one
            var validatedAddress = _addressService.CloneAddress(address);
            validatedAddress.City = validatedAddressInfo.city;
            validatedAddress.CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(validatedAddressInfo.country))?.Id;
            validatedAddress.Address1 = validatedAddressInfo.line1;
            validatedAddress.Address2 = validatedAddressInfo.line2;
            validatedAddress.ZipPostalCode = validatedAddressInfo.postalCode;
            validatedAddress.StateProvinceId = (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(validatedAddressInfo.region))?.Id;

            //try to find an existing address with the same values
            var existingAddress = _addressService.FindAddress((await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
                validatedAddress.FirstName, validatedAddress.LastName, validatedAddress.PhoneNumber,
                validatedAddress.Email, validatedAddress.FaxNumber, validatedAddress.Company,
                validatedAddress.Address1, validatedAddress.Address2, validatedAddress.City,
                validatedAddress.County, validatedAddress.StateProvinceId, validatedAddress.ZipPostalCode,
                validatedAddress.CountryId, validatedAddress.CustomAttributes);

            //if the found address is the same as address to validate, nothing to display
            if (address.Id == existingAddress?.Id)
                return Content(string.Empty);

            //otherwise display to customer a confirmation dialog about address updating
            var model = new AddressValidationModel();
            if (existingAddress == null)
            {
                await _addressService.InsertAddressAsync(validatedAddress);
                model.AddressId = validatedAddress.Id;
                model.IsNewAddress = true;
            }
            else
                model.AddressId = existingAddress.Id;

            async Task<string> getAddressLineAsync(Address address) =>
                WebUtility.HtmlEncode($"{(!string.IsNullOrEmpty(address.Address1) ? $"{address.Address1}, " : string.Empty)}" +
                    $"{(!string.IsNullOrEmpty(address.Address2) ? $"{address.Address2}, " : string.Empty)}" +
                    $"{(!string.IsNullOrEmpty(address.City) ? $"{address.City}, " : string.Empty)}" +
                    $"{(await _stateProvinceService.GetStateProvinceByAddressAsync(address) is StateProvince stateProvince ? $"{stateProvince.Name}, " : string.Empty)}" +
                    $"{(await _countryService.GetCountryByAddressAsync(address) is Country country ? $"{country.Name}, " : string.Empty)}" +
                    $"{(!string.IsNullOrEmpty(address.ZipPostalCode) ? $"{address.ZipPostalCode}, " : string.Empty)}"
                    .TrimEnd(' ').TrimEnd(','));

            model.Message = string.Format(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.AddressValidation.Confirm"),
                await getAddressLineAsync(address), await getAddressLineAsync(existingAddress ?? validatedAddress));

            return View("~/Plugins/Tax.Avalara/Views/Checkout/AddressValidation.cshtml", model);
        }

        #endregion
    }
}