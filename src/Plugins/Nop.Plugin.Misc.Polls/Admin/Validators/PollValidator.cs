using FluentValidation;
using Nop.Plugin.Misc.Polls.Admin.Models;
using Nop.Plugin.Misc.Polls.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Polls.Admin.Validators;

public class PollValidator : BaseNopValidator<PollModel>
{
    public PollValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Polls.Fields.Name.Required"));

        SetDatabaseValidationRules<Poll>();
    }
}