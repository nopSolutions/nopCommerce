using FluentValidation;
using Nop.Web.Areas.Admin.Models.Forums;
using Nop.Core.Domain.Forums;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Forums
{
    public partial class ForumGroupValidator : BaseNopValidator<ForumGroupModel>
    {
        public ForumGroupValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Forums.ForumGroup.Fields.Name.Required"));

            SetDatabaseValidationRules<ForumGroup>(dataProvider);
        }
    }
}