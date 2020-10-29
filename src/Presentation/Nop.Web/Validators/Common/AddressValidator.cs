using System.Linq;
using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common
{
    public partial class AddressValidator : BaseNopValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            AddressSettings addressSettings,
            CustomerSettings customerSettings)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Address.Fields.FirstName.Required").Result);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Address.Fields.LastName.Required").Result);
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Address.Fields.Email.Required").Result);
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);
            if (addressSettings.CountryEnabled)
            {
                RuleFor(x => x.CountryId)
                    .NotNull()
                    .WithMessage(localizationService.GetResourceAsync("Address.Fields.Country.Required").Result);
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResourceAsync("Address.Fields.Country.Required").Result);
            }
            if (addressSettings.CountryEnabled && addressSettings.StateProvinceEnabled)
            {
                RuleFor(x => x.StateProvinceId).Must((x, context) =>
                {
                    //does selected country has states?
                    var countryId = x.CountryId ?? 0;
                    var hasStates = stateProvinceService.GetStateProvincesByCountryIdAsync(countryId).Result.Any();

                    if (hasStates)
                    {
                        //if yes, then ensure that state is selected
                        if (!x.StateProvinceId.HasValue || x.StateProvinceId.Value == 0)
                            return false;
                    }

                    return true;
                }).WithMessage(localizationService.GetResourceAsync("Address.Fields.StateProvince.Required").Result);
            }
            if (addressSettings.CompanyRequired && addressSettings.CompanyEnabled)
            {
                RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Company.Required").Result);
            }
            if (addressSettings.StreetAddressRequired && addressSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.Address1).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.StreetAddress.Required").Result);
            }
            if (addressSettings.StreetAddress2Required && addressSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.Address2).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.StreetAddress2.Required").Result);
            }
            if (addressSettings.ZipPostalCodeRequired && addressSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.ZipPostalCode.Required").Result);
            }
            if (addressSettings.CountyEnabled && addressSettings.CountyRequired)
            {
                RuleFor(x => x.County).NotEmpty().WithMessage(localizationService.GetResourceAsync("Address.Fields.County.Required").Result);
            }
            if (addressSettings.CityRequired && addressSettings.CityEnabled)
            {
                RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.City.Required").Result);
            }
            if (addressSettings.PhoneRequired && addressSettings.PhoneEnabled)
            {
                RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Phone.Required").Result);
            }
            if (addressSettings.PhoneEnabled)
            {
                RuleFor(x => x.PhoneNumber).IsPhoneNumber(customerSettings).WithMessage(localizationService.GetResourceAsync("Account.Fields.Phone.NotValid").Result);
            }
            if (addressSettings.FaxRequired && addressSettings.FaxEnabled)
            {
                RuleFor(x => x.FaxNumber).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Fax.Required").Result);
            }
        }
    }
}