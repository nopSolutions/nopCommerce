using System;
using System.Linq;
using FluentValidation;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public partial class RegisterValidator : BaseNopValidator<RegisterModel>
    {
        public RegisterValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            CustomerSettings customerSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Email.Required").Result);
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);

            if (customerSettings.EnteringEmailTwice)
            {
                RuleFor(x => x.ConfirmEmail).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.ConfirmEmail.Required").Result);
                RuleFor(x => x.ConfirmEmail).EmailAddress().WithMessage(localizationService.GetResourceAsync("Common.WrongEmail").Result);
                RuleFor(x => x.ConfirmEmail).Equal(x => x.Email).WithMessage(localizationService.GetResourceAsync("Account.Fields.Email.EnteredEmailsDoNotMatch").Result);
            }

            if (customerSettings.UsernamesEnabled)
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Username.Required").Result);
                RuleFor(x => x.Username).IsUsername(customerSettings).WithMessage(localizationService.GetResourceAsync("Account.Fields.Username.NotValid").Result);
            }

            if (customerSettings.FirstNameEnabled && customerSettings.FirstNameRequired)
            {
                RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.FirstName.Required").Result);
            }
            if (customerSettings.LastNameEnabled && customerSettings.LastNameRequired)
            {
                RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.LastName.Required").Result);
            }

            //Password rule
            RuleFor(x => x.Password).IsPassword(localizationService, customerSettings);

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.ConfirmPassword.Required").Result);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(localizationService.GetResourceAsync("Account.Fields.Password.EnteredPasswordsDoNotMatch").Result);

            //form fields
            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResourceAsync("Account.Fields.Country.Required").Result);
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {
                RuleFor(x => x.StateProvinceId).Must((x, context) =>
                {
                    //does selected country have states?
                    var hasStates = stateProvinceService.GetStateProvincesByCountryIdAsync(x.CountryId).Result.Any();
                    if (hasStates)
                    {
                        //if yes, then ensure that a state is selected
                        if (x.StateProvinceId == 0)
                            return false;
                    }

                    return true;
                }).WithMessage(localizationService.GetResourceAsync("Account.Fields.StateProvince.Required").Result);
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
                }).WithMessage(localizationService.GetResourceAsync("Account.Fields.DateOfBirth.Required").Result);

                //minimum age
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    if (dateOfBirth.HasValue && customerSettings.DateOfBirthMinimumAge.HasValue &&
                        CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) <
                        customerSettings.DateOfBirthMinimumAge.Value)
                        return false;

                    return true;
                }).WithMessage(string.Format(localizationService.GetResourceAsync("Account.Fields.DateOfBirth.MinimumAge").Result, customerSettings.DateOfBirthMinimumAge));
            }
            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Company.Required").Result);
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.StreetAddress).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.StreetAddress.Required").Result);
            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.StreetAddress2).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.StreetAddress2.Required").Result);
            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.ZipPostalCode.Required").Result);
            }
            if (customerSettings.CountyRequired && customerSettings.CountyEnabled)
            {
                RuleFor(x => x.County).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.County.Required").Result);
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.City.Required").Result);
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Phone.Required").Result);
            }
            if (customerSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).IsPhoneNumber(customerSettings).WithMessage(localizationService.GetResourceAsync("Account.Fields.Phone.NotValid").Result);
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.Fields.Fax.Required").Result);
            }
        }
    }
}