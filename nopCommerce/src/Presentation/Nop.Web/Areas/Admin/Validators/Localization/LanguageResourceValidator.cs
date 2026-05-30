using FluentValidation;
using Nop.Core.Domain.Localization;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Localization;

public partial class LanguageResourceValidator : BaseNopValidator<LocaleResourceModel>
{
    public LanguageResourceValidator(ILocalizationService localizationService)
    {
        //if validation without this set rule is applied, in this case nothing will be validated
        //it's used to prevent auto-validation of child models
        RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
        {
            RuleFor(model => model.ResourceName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Name.Required"));

            RuleFor(model => model.ResourceValue)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Languages.Resources.Fields.Value.Required"));

            SetDatabaseValidationRules<LocaleStringResource>();
        });
    }
}