using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        private async Task AddAddress(Customer customer, Address address, bool isDefault)
        {
            if (await _addressService.IsAddressValidAsync(address))
            {
                await _addressService.InsertAddressAsync(address);

                await _customerService.InsertCustomerAddressAsync(customer, address);

                if (isDefault)
                {
                    customer.BillingAddressId = address.Id;
                    customer.ShippingAddressId = address.Id;

                    await _customerService.UpdateCustomerAsync(customer);
                }
            }
        }

        public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
        {
            var customer = eventMessage.Customer;
            if(IsServiceTitanEmail(customer.Email))
            {
                await AddAddress(customer, new Address
                {
                    FirstName = "Melik Adamyan 2/2",
                    LastName = "3rd floor",
                    Email = customer.Email,
                    Company = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute),
                    CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync("AM")).Id,
                    StateProvinceId = null,
                    County = "",
                    City = "Yerevan",
                    Address1 = "Melik Adamyan 2/2",
                    Address2 = "3rd floor",
                    ZipPostalCode = "0010",
                    PhoneNumber = "+374-99-094095",
                    FaxNumber = "",
                    CreatedOnUtc = customer.CreatedOnUtc
                }, true);

                await AddAddress(customer, new Address
                {
                    FirstName = "Leo 1",
                    LastName = "2nd floor",
                    Email = customer.Email,
                    Company = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute),
                    CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync("AM")).Id,
                    StateProvinceId = null,
                    County = "",
                    City = "Yerevan",
                    Address1 = "Leo 1",
                    Address2 = "2nd floor",
                    ZipPostalCode = "0010",
                    PhoneNumber = "+374-99-094095",
                    FaxNumber = "",
                    CreatedOnUtc = customer.CreatedOnUtc
                }, false);
            }
        }
    }
}
