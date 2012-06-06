using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Customers
{
    public class CustomerValidator : AbstractValidator<CustomerModel>
    {
        public CustomerValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            //form fields
            RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Company.Required"))
                .When(x => customerSettings.CompanyRequired && customerSettings.CompanyEnabled);
            RuleFor(x => x.StreetAddress).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.StreetAddress.Required"))
                .When(x => customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled);
            RuleFor(x => x.StreetAddress2).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.StreetAddress2.Required"))
                .When(x => customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled);
            RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.ZipPostalCode.Required"))
                .When(x => customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled);
            RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.City.Required"))
                .When(x => customerSettings.CityRequired && customerSettings.CityEnabled);
            RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Phone.Required"))
                .When(x => customerSettings.PhoneRequired && customerSettings.PhoneEnabled);
            RuleFor(x => x.Fax).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Fax.Required"))
                .When(x => customerSettings.FaxRequired && customerSettings.FaxEnabled);
        }
    }
}