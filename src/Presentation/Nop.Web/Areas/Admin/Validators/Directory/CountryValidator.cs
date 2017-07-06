using FluentValidation;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Directory
{
    public partial class CountryValidator : BaseNopValidator<CountryModel>
    {
        public CountryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.Fields.Name.Required"));
            RuleFor(p => p.Name).Length(1, 100);

            RuleFor(x => x.TwoLetterIsoCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Required"));
            RuleFor(x => x.TwoLetterIsoCode)
                .Length(2)
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Length"));

            RuleFor(x => x.ThreeLetterIsoCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Required"));
            RuleFor(x => x.ThreeLetterIsoCode)
                .Length(3)
                .WithMessage(localizationService.GetResource("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Length"));
        }
    }
}