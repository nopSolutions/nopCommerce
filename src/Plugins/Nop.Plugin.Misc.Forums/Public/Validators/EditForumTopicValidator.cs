using FluentValidation;
using Nop.Plugin.Misc.Forums.Public.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Forums.Public.Validators;

public class EditForumTopicValidator : BaseNopValidator<EditForumTopicModel>
{
    public EditForumTopicValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Forums.TopicSubjectCannotBeEmpty"));
        RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Forums.TextCannotBeEmpty"));
    }
}