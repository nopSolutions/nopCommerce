using FluentValidation;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Directory
{
    public partial class CountryValidator : BaseNopValidator<CountryModel>
    {
        public CountryValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.Name.Required").Result);
            RuleFor(p => p.Name).Length(1, 100);

            RuleFor(x => x.TwoLetterIsoCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Required").Result);
            RuleFor(x => x.TwoLetterIsoCode)
                .Length(2)
                .WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.TwoLetterIsoCode.Length").Result);

            RuleFor(x => x.ThreeLetterIsoCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Required").Result);
            RuleFor(x => x.ThreeLetterIsoCode)
                .Length(3)
                .WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode.Length").Result);

            SetDatabaseValidationRules<Country>(dataProvider);
        }
    }
}