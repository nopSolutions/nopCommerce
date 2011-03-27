using System.Globalization;
using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class LanguageValidator : AbstractValidator<LanguageModel>
    {
		#region Constructors 

        public LanguageValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Common.Validation.Required"));
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

		#endregion Constructors 
    }
}