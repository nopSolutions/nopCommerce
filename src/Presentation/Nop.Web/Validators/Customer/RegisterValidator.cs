using FluentValidation;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer;

public partial class RegisterValidator : BaseNopValidator<RegisterModel>
{
    public RegisterValidator(ILocalizationService localizationService,
        IStateProvinceService stateProvinceService,
        CustomerSettings customerSettings,
        TaxSettings taxSettings)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Account.Fields.Email.Required");
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Common.WrongEmail");

        if (customerSettings.EnteringEmailTwice)
        {
            RuleFor(x => x.ConfirmEmail).NotEmpty().WithMessage("Account.Fields.ConfirmEmail.Required");
            RuleFor(x => x.ConfirmEmail)
                .IsEmailAddress()
                .WithMessage("Common.WrongEmail");
            RuleFor(x => x.ConfirmEmail).Equal(x => x.Email).WithMessage("Account.Fields.Email.EnteredEmailsDoNotMatch");
        }

        if (customerSettings.UsernamesEnabled)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Account.Fields.Username.Required");
            RuleFor(x => x.Username).IsUsername(customerSettings).WithMessage("Account.Fields.Username.NotValid");
        }

        if (customerSettings.FirstNameEnabled && customerSettings.FirstNameRequired)
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Account.Fields.FirstName.Required");
        }
        if (customerSettings.LastNameEnabled && customerSettings.LastNameRequired)
        {
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Account.Fields.LastName.Required");
        }

        //Password rule
        RuleFor(x => x.Password).IsPassword(localizationService, customerSettings);

        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Account.Fields.ConfirmPassword.Required");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Account.Fields.Password.EnteredPasswordsDoNotMatch");

        //form fields
        if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
        {
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage("Account.Fields.Country.Required");
        }
        if (customerSettings.CountryEnabled &&
            customerSettings.StateProvinceEnabled &&
            customerSettings.StateProvinceRequired)
        {
            RuleFor(x => x.StateProvinceId).MustAsync(async (x, context, cancellation) =>
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
            }).WithMessage("Account.Fields.StateProvince.Required");
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
            }).WithMessage("Account.Fields.DateOfBirth.Required");

            //minimum age
            RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
            {
                var dateOfBirth = x.ParseDateOfBirth();
                if (dateOfBirth.HasValue && customerSettings.DateOfBirthMinimumAge.HasValue &&
                    CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) <
                    customerSettings.DateOfBirthMinimumAge.Value)
                    return false;

                return true;
            }).WithMessage(string.Format("Account.Fields.DateOfBirth.MinimumAge", customerSettings.DateOfBirthMinimumAge));
        }
        if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
        {
            RuleFor(x => x.Company).NotEmpty().WithMessage("Account.Fields.Company.Required");
        }
        if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
        {
            RuleFor(x => x.StreetAddress).NotEmpty().WithMessage("Account.Fields.StreetAddress.Required");
        }
        if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
        {
            RuleFor(x => x.StreetAddress2).NotEmpty().WithMessage("Account.Fields.StreetAddress2.Required");
        }
        if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
        {
            RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage("Account.Fields.ZipPostalCode.Required");
        }
        if (customerSettings.CountyRequired && customerSettings.CountyEnabled)
        {
            RuleFor(x => x.County).NotEmpty().WithMessage("Account.Fields.County.Required");
        }
        if (customerSettings.CityRequired && customerSettings.CityEnabled)
        {
            RuleFor(x => x.City).NotEmpty().WithMessage("Account.Fields.City.Required");
        }
        if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
        {
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Account.Fields.Phone.Required");
        }
        if (customerSettings.PhoneEnabled)
        {
            RuleFor(x => x.Phone).IsPhoneNumber(customerSettings).WithMessage("Account.Fields.Phone.NotValid");
        }
        if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
        {
            RuleFor(x => x.Fax).NotEmpty().WithMessage("Account.Fields.Fax.Required");
        }

        //Tax settings
        if (taxSettings.EuVatEnabled && taxSettings.EuVatRequired)
        {
            RuleFor(x => x.VatNumber).NotEmpty().WithMessage("Account.Fields.VatNumber.Required");
        }

    }
}