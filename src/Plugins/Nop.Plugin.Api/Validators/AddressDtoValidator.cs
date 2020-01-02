using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class AddressDtoValidator : BaseDtoValidator<AddressDto>
    {

        #region Constructors

        public AddressDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetFirstNameRule();
            SetLastNameRule();
            SetEmailRule();

            SetAddress1Rule();
            SetCityRule();
            SetZipPostalCodeRule();
            SetCountryIdRule();
            SetPhoneNumberRule();
        }

        #endregion

        #region Private Methods

        private void SetAddress1Rule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(a => a.Address1, "address1 required", "address1");
        }

        private void SetCityRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(a => a.City, "city required", "city");
        }

        private void SetCountryIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(a => a.CountryId, "country_id required", "country_id");
        }

        private void SetEmailRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(a => a.Email, "email required", "email");
        }

        private void SetFirstNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(a => a.FirstName, "first_name required", "first_name");
        }

        private void SetLastNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(a => a.LastName, "first_name required", "last_name");
        }

        private void SetPhoneNumberRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(a => a.PhoneNumber, "phone_number required", "phone_number");
        }

        private void SetZipPostalCodeRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(a => a.ZipPostalCode, "zip_postal_code required", "zip_postal_code");
        }

        #endregion

    }
}