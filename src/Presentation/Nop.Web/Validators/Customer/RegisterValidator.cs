using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public class RegisterValidator : AbstractValidator<RegisterModel>
    {
        public RegisterValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.FirstName.Required"));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.LastName.Required"));


            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Password.Required"));
            RuleFor(x => x.Password).Length(customerSettings.PasswordMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Account.Fields.Password.LengthValidation"), customerSettings.PasswordMinLength));
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.ConfirmPassword.Required"));
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(localizationService.GetResource("Account.Fields.Password.EnteredPasswordsDoNotMatch"));


            //form fields
            RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Company.Required"))
                .When(x => customerSettings.CompanyRequired && customerSettings.CompanyEnabled);
            RuleFor(x => x.StreetAddress).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress.Required"))
                .When(x => customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled);
            RuleFor(x => x.StreetAddress2).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress2.Required"))
                .When(x => customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled);
            RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.ZipPostalCode.Required"))
                .When(x => customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled);
            RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.City.Required"))
                .When(x => customerSettings.CityRequired && customerSettings.CityEnabled);
            RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Phone.Required"))
                .When(x => customerSettings.PhoneRequired && customerSettings.PhoneEnabled);
            RuleFor(x => x.Fax).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Fax.Required"))
                .When(x => customerSettings.FaxRequired && customerSettings.FaxEnabled);
        }}
}