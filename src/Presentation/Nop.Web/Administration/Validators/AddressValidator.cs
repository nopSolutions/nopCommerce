using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class AddressValidator : AbstractValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService)
        {
            //UNDONE add localization
            RuleFor(x => x.FirstName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.FirstName.Validation"))
                .When(x => !x.FirstNameDisabled);
            RuleFor(x => x.LastName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.LastName.Validation"))
                .When(x => !x.LastNameDisabled);
            RuleFor(x => x.Email)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.Email.Validation"))
                .When(x => !x.EmailDisabled);
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.Country.Validation"))
                .When(x => !x.CountryDisabled);
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.Country.Validation"))
                .When(x => !x.CountryDisabled);
            RuleFor(x => x.City)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.City.Validation"))
                .When(x => !x.CityDisabled);
            RuleFor(x => x.ZipPostalCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.ZipPostalCode.Validation"))
                .When(x => !x.ZipPostalCodeDisabled);
            RuleFor(x => x.PhoneNumber)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.PhoneNumber.Validation"))
                .When(x => !x.PhoneNumberDisabled);
        }
    }
}