using FluentValidation;
using Nop.Plugin.Misc.Forums.Admin.Models;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Misc.Forums.Admin.Validators;

public class ForumValidator : BaseNopValidator<ForumModel>
{
    public ForumValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Forums.Forum.Fields.Name.Required"));
        RuleFor(x => x.ForumGroupId).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Misc.Forums.Forum.Fields.ForumGroupId.Required"));

        SetDatabaseValidationRules<Forum>();
    }
}