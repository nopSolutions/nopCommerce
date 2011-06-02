using System.Globalization;
using FluentValidation;
using Nop.Admin.Models.Localization;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Localization
{
    public class LanguageValidator : AbstractValidator<LanguageModel>
    {
        public LanguageValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Location.Languages.Fields.Name.Required"));
            RuleFor(x => x.LanguageCulture)
                .Must(x =>
                          {
                              try
                              {
                                  var culture = new CultureInfo(x);
                                  return culture != null;
                              }
                              catch
                              {
                                  return false;
                              }
                          })
                .WithMessage(localizationService.GetResource("Admin.Configuration.Location.Languages.Fields.LanguageCulture.Validation"));
        }
    }
}