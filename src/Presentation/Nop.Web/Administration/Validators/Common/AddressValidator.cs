using FluentValidation;
using Nop.Admin.Models.Common;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Common
{
    public class AddressValidator : AbstractValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.FirstName.Required"))
                .When(x => !x.FirstNameDisabled);
            RuleFor(x => x.LastName)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.LastName.Required"))
                .When(x => !x.LastNameDisabled);
            RuleFor(x => x.Email)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Email.Required"))
                .When(x => !x.EmailDisabled);
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"))
                .When(x => !x.EmailDisabled);
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Country.Required"))
                .When(x => !x.CountryDisabled);
            RuleFor(x => x.CountryId)
                .NotEqual(0)
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Country.Required"))
                .When(x => !x.CountryDisabled);
            RuleFor(x => x.City)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.City.Required"))
                .When(x => !x.CityDisabled);
            RuleFor(x => x.Address1)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.Address1.Required"))
                .When(x => !x.Address1Disabled);
            RuleFor(x => x.ZipPostalCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.ZipPostalCode.Required"))
                .When(x => !x.ZipPostalCodeDisabled);
            RuleFor(x => x.PhoneNumber)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.Address.Fields.PhoneNumber.Required"))
                .When(x => !x.PhoneNumberDisabled);
        }
    }
}