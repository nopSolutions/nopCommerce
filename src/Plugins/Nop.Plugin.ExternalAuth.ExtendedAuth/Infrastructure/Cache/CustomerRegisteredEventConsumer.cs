using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Service
{
    public class CustomerRegisteredEventConsumer
        : IConsumer<CustomerRegisteredEvent>
    {
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;

        public CustomerRegisteredEventConsumer(ICustomerService customerService, IAddressService addressService,
            IGenericAttributeService genericAttributeService, ICountryService countryService)
        {
            _customerService = customerService;
            _addressService = addressService;
            _genericAttributeService = genericAttributeService;
            _countryService = countryService;
        }

        private bool IsServiceTitanEmail(string email)
        {
            return email.EndsWith("@servicetitan.com", StringComparison.InvariantCultureIgnoreCase);
        }

        public void HandleEvent(CustomerRegisteredEvent eventMessage)
        {
            var customer = eventMessage.Customer;
            if(IsServiceTitanEmail(customer.Email))
            {
                var defaultAddress = new Address
                {
                    FirstName = "Melik Adamyan 2/2",
                    LastName = "2nd floor",
                    Email = customer.Email,
                    Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                    CountryId = _countryService.GetCountryByTwoLetterIsoCode("AM").Id,
                    StateProvinceId = null,
                    County = "",
                    City = "Yerevan",
                    Address1 = "Melik Adamyan 2/2",
                    Address2 = "2nd floor",
                    ZipPostalCode = "0010",
                    PhoneNumber = "+374-99-094095",
                    FaxNumber = "",
                    CreatedOnUtc = customer.CreatedOnUtc
                };

                if (_addressService.IsAddressValid(defaultAddress))
                {
                    _addressService.InsertAddress(defaultAddress);

                    _customerService.InsertCustomerAddress(customer, defaultAddress);

                    customer.BillingAddressId = defaultAddress.Id;
                    customer.ShippingAddressId = defaultAddress.Id;

                    _customerService.UpdateCustomer(customer);
                }
            }
        }
    }
}
