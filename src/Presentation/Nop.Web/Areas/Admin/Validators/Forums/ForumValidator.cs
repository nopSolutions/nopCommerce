using FluentValidation;
using Nop.Core.Domain.Forums;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Forums;

public partial class ForumValidator : BaseNopValidator<ForumModel>
{
    public ForumValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Fields.Name.Required"));
        RuleFor(x => x.ForumGroupId).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId.Required"));

        SetDatabaseValidationRules<Forum>();
    }
}