using Iyzipay.Model;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Iyzico.Services
{
    public class PaymentIyzicoService : IPaymentIyzicoService
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public PaymentIyzicoService(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IAddressService addressService,
            ICountryService countryService)
        {
            _customerService = customerService;
            _addressService = addressService;
            _countryService = countryService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public virtual async Task<Buyer> GetBuyer(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);

            var customerName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);

            var customerSurName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);

            var customerIdentityNumber = await _genericAttributeService.GetAttributeAsync<string>(customer, "IdentityNumber");
            if (string.IsNullOrEmpty(customerIdentityNumber))
                customerIdentityNumber = "11111111111";

            var billingAddress = await _addressService.GetAddressByIdAsync(customer.BillingAddressId ?? 0);
            if (billingAddress == null)
                throw new NopException("Müşteri fatura adresi ayarlanmadı!");

            var country = await _countryService.GetCountryByIdAsync(billingAddress.CountryId ?? 0);
            if (country == null)
                throw new NopException("Fatura adresi ülkesi ayarlanmadı!");

            var buyer = new Buyer
            {
                Id = customer.CustomerGuid.ToString(),
                Name = customerName,
                Surname = customerSurName,
                Email = customer.Email,
                IdentityNumber = customerIdentityNumber,
                RegistrationAddress = billingAddress.Address1,
                Ip = customer.LastIpAddress,
                City = billingAddress.City,
                Country = country.Name,
                ZipCode = billingAddress.ZipPostalCode,
                GsmNumber = billingAddress.PhoneNumber,
                RegistrationDate = IyzicoHelper.ToIyzicoDateFormat(customer.CreatedOnUtc),
                LastLoginDate = IyzicoHelper.ToIyzicoDateFormat(customer.LastLoginDateUtc)
            };

            return buyer;
        }

        public virtual async Task<Address> GetAddress(Core.Domain.Common.Address address)
        {
            var country = await _countryService.GetCountryByIdAsync(address.CountryId ?? 0);
            if (country == null)
                throw new NopException("Fatura adresi ülkesi ayarlanmadı!");

            return new Address
            {
                ContactName = $"{address.FirstName} {address.LastName}",
                City = address.City,
                Country = country.Name,
                Description = address.Address1,
                ZipCode = address.ZipPostalCode
            };
        }

        #endregion
    }
}