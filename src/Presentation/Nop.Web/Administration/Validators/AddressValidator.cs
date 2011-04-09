using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class AddressValidator : AbstractValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService)
        {
            //UNDONE add required validation based on [Fields]Disabled/[Fields]Required proeprties (e.g, FirstNameDisabled)
            //RuleFor(x => x.CountryId).NotNull().WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.Country.Validation"));
            //RuleFor(x => x.CountryId).NotEqual(0).WithMessage(localizationService.GetResource("Admin.Common.Address.Fields.Country.Validation"));
        }}
}