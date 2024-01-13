using FluentValidation;
using Nop.Core.Domain.Forums;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Forums;

public partial class ForumGroupValidator : BaseNopValidator<ForumGroupModel>
{
    public ForumGroupValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Forums.ForumGroup.Fields.Name.Required"));

        SetDatabaseValidationRules<ForumGroup>();
    }
}