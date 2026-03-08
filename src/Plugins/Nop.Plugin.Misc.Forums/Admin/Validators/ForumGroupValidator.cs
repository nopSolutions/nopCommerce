using FluentValidation;
using Nop.Plugin.Misc.Forums.Admin.Models;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Forums.Admin.Validators;

public class ForumGroupValidator : BaseNopValidator<ForumGroupModel>
{
    public ForumGroupValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Forums.ForumGroup.Fields.Name.Required"));

        SetDatabaseValidationRules<ForumGroup>();
    }
}