using FluentValidation;
using Nop.Plugin.Misc.Polls.Admin.Models;
using Nop.Plugin.Misc.Polls.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Polls.Admin.Validators;

public class PollAnswerValidator : BaseNopValidator<PollAnswerModel>
{
    public PollAnswerValidator(ILocalizationService localizationService)
    {
        //if validation without this set rule is applied, in this case nothing will be validated
        //it's used to prevent auto-validation of child models
        RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
        {
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Polls.Answers.Fields.Name.Required"));

            SetDatabaseValidationRules<PollAnswer>();
        });
    }
}