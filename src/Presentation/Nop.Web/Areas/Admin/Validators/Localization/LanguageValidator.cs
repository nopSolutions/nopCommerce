using System.Globalization;
using FluentValidation;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Localization
{
    public partial class LanguageValidator : BaseNopValidator<LanguageModel>
    {
        public LanguageValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.Name.Required").Result);
            RuleFor(x => x.LanguageCulture)
                .Must(x =>
                          {
                              try
                              {
                                  //let's try to create a CultureInfo object
                                  //if "DisplayLocale" is wrong, then exception will be thrown
                                  var unused = new CultureInfo(x);
                                  return true;
                              }
                              catch
                              {
                                  return false;
                              }
                          })
                .WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.LanguageCulture.Validation").Result);

            RuleFor(x => x.UniqueSeoCode).NotEmpty().WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.UniqueSeoCode.Required").Result);
            RuleFor(x => x.UniqueSeoCode).Length(2).WithMessage(localizationService.GetResourceAsync("Admin.Configuration.Languages.Fields.UniqueSeoCode.Length").Result);

            SetDatabaseValidationRules<Language>(dataProvider, "UniqueSeoCode");
        }
    }
}