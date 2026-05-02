using FluentValidation;
using Nop.Plugin.Misc.Forums.Public.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Forums.Public.Validators;

public class EditForumPostValidator : BaseNopValidator<EditForumPostModel>
{
    public EditForumPostValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Text).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Forums.TextCannotBeEmpty"));
    }
}