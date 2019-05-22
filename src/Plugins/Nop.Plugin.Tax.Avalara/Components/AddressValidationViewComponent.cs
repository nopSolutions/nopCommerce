using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Avalara.AvaTax.RestClient;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.Avalara.Models.Checkout;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
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
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public AddressValidationViewComponent(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            IAddressService addressService,
            ICountryService countryService,
            ILocalizationService localizationService,
            ILogger logger,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxPluginManager taxPluginManager,
            IWorkContext workContext,
            TaxSettings taxSettings)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _addressService = addressService;
            _countryService = countryService;
            _localizationService = localizationService;
            _logger = logger;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxPluginManager = taxPluginManager;
            _workContext = workContext;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get address line
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Address line</returns>
        private string GetAddressLine(Address address)
        {
            return WebUtility.HtmlEncode($"{(!string.IsNullOrEmpty(address.Address1) ? $"{address.Address1}, " : string.Empty)}" +
                $"{(!string.IsNullOrEmpty(address.Address2) ? $"{address.Address2}, " : string.Empty)}" +
                $"{(!string.IsNullOrEmpty(address.City) ? $"{address.City}, " : string.Empty)}" +
                $"{(!string.IsNullOrEmpty(address.StateProvince?.Name) ? $"{address.StateProvince.Name}, " : string.Empty)}" +
                $"{(!string.IsNullOrEmpty(address.Country?.Name) ? $"{address.Country.Name}, " : string.Empty)}" +
                $"{(!string.IsNullOrEmpty(address.ZipPostalCode) ? $"{address.ZipPostalCode}, " : string.Empty)}"
                .TrimEnd(' ').TrimEnd(','));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id))
                return Content(string.Empty);

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(PublicWidgetZones.CheckoutConfirmTop) && !widgetZone.Equals(PublicWidgetZones.OpCheckoutConfirmTop))
                return Content(string.Empty);

            //ensure thet address validation is enabled
            if (!_avalaraTaxSettings.ValidateAddress)
                return Content(string.Empty);

            //validate entered by customer addresses only
            var address = _taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress
                ? _workContext.CurrentCustomer.BillingAddress
                : _taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress
                ? _workContext.CurrentCustomer.ShippingAddress
                : null;
            if (address == null)
                return Content(string.Empty);

            //validate address
            var validationResult = _avalaraTaxManager.ValidateAddress(new AddressValidationInfo
            {
                city = CommonHelper.EnsureMaximumLength(address.City, 50),
                country = CommonHelper.EnsureMaximumLength(address.Country?.TwoLetterIsoCode, 2),
                line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
                line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
                postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
                region = CommonHelper.EnsureMaximumLength(address.StateProvince?.Abbreviation, 3),
                textCase = TextCase.Mixed
            });

            //whether there are errors in validation result
            var errorDetails = validationResult.messages?
                .Where(message => message.severity.Equals("Error", StringComparison.InvariantCultureIgnoreCase))
                .Select(message => message.details) ?? new List<string>();
            if (errorDetails.Any())
            {
                var errorMessage = errorDetails.Aggregate(string.Empty, (message, errorDetail) => $"{message}{errorDetail}; ");

                //log errors
                _logger.Error($"Avalara tax provider error. {errorMessage}", customer: _workContext.CurrentCustomer);

                //and display error message to customer
                return View("~/Plugins/Tax.Avalara/Views/Checkout/AddressValidation.cshtml", new AddressValidationModel
                {
                    Message = string.Format(_localizationService.GetResource("Plugins.Tax.Avalara.AddressValidation.Error"), WebUtility.HtmlEncode(errorMessage)),
                    IsError = true
                });
            }

            //if there are no errors and no validated addresses, nothing to display
            if (!validationResult.validatedAddresses?.Any() ?? true)
                return Content(string.Empty);

            //get validated address info
            var validatedAddressInfo = validationResult.validatedAddresses.FirstOrDefault();

            //create new address as a copy of address to validate and with details of the validated one
            var validatedAddress = address.Clone() as Address;
            validatedAddress.City = validatedAddressInfo.city;
            validatedAddress.Country = _countryService.GetCountryByTwoLetterIsoCode(validatedAddressInfo.country);
            validatedAddress.Address1 = validatedAddressInfo.line1;
            validatedAddress.Address2 = validatedAddressInfo.line2;
            validatedAddress.ZipPostalCode = validatedAddressInfo.postalCode;
            validatedAddress.StateProvince = _stateProvinceService.GetStateProvinceByAbbreviation(validatedAddressInfo.region);

            //try to find an existing address with the same values
            var existingAddress = _addressService.FindAddress(_workContext.CurrentCustomer.Addresses.ToList(),
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
                _addressService.InsertAddress(validatedAddress);
                model.AddressId = validatedAddress.Id;
                model.IsNewAddress = true;
            }
            else
                model.AddressId = existingAddress.Id;

            model.Message = string.Format(_localizationService.GetResource("Plugins.Tax.Avalara.AddressValidation.Confirm"),
                GetAddressLine(address), GetAddressLine(existingAddress ?? validatedAddress));

            return View("~/Plugins/Tax.Avalara/Views/Checkout/AddressValidation.cshtml", model);
        }

        #endregion
    }
}