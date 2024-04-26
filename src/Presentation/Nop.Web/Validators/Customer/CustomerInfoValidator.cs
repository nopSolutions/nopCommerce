using FluentValidation;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer;

public partial class CustomerInfoValidator : BaseNopValidator<CustomerInfoModel>
{
    public CustomerInfoValidator(ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        CustomerSettings customerSettings)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Email.Required"));
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));

        if (customerSettings.FirstNameEnabled && customerSettings.FirstNameRequired)
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.FirstName.Required"));
        }
        if (customerSettings.LastNameEnabled && customerSettings.LastNameRequired)
        {
            RuleFor(x => x.LastName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.LastName.Required"));
        }

        if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Username.Required"));
            RuleFor(x => x.Username).IsUsername(customerSettings).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Username.NotValid"));
        }

        //form fields
        if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
        {
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Country.Required"));
        }
        if (customerSettings.CountryEnabled &&
            customerSettings.StateProvinceEnabled &&
            customerSettings.StateProvinceRequired)
        {
            RuleFor(x => x.StateProvinceId).MustAwait(async (x, context) =>
            {
                //does selected country have states?
                var hasStates = (await stateProvinceService.GetStateProvincesByCountryIdAsync(x.CountryId)).Any();
                if (hasStates)
                {
                    //if yes, then ensure that a state is selected
                    if (x.StateProvinceId == 0)
                        return false;
                }

                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StateProvince.Required"));
        }
        if (customerSettings.DateOfBirthEnabled && customerSettings.DateOfBirthRequired)
        {
            //entered?
            RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
            {
                var dateOfBirth = x.ParseDateOfBirth();
                if (!dateOfBirth.HasValue)
                    return false;

                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.DateOfBirth.Required"));

            //minimum age
            RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
            {
                var dateOfBirth = x.ParseDateOfBirth();
                if (dateOfBirth.HasValue && customerSettings.DateOfBirthMinimumAge.HasValue &&
                    CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) <
                    customerSettings.DateOfBirthMinimumAge.Value)
                    return false;

                return true;
            }).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.DateOfBirth.MinimumAge"), customerSettings.DateOfBirthMinimumAge);
        }
        if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
        {
            RuleFor(x => x.Company).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Company.Required"));
        }
        if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
        {
            RuleFor(x => x.StreetAddress).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StreetAddress.Required"));
        }
        if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
        {
            RuleFor(x => x.StreetAddress2).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StreetAddress2.Required"));
        }
        if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
        {
            RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.ZipPostalCode.Required"));
        }
        if (customerSettings.CountyRequired && customerSettings.CountyEnabled)
        {
            RuleFor(x => x.County).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.County.Required"));
        }
        if (customerSettings.CityRequired && customerSettings.CityEnabled)
        {
            RuleFor(x => x.City).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.City.Required"));
        }
        if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
        {
            RuleFor(x => x.Phone).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.Required"));
        }
        if (customerSettings.PhoneEnabled)
        {
            RuleFor(x => x.Phone).IsPhoneNumber(customerSettings).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.NotValid"));
        }
        if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
        {
            RuleFor(x => x.Fax).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Fax.Required"));
        }
    }
}