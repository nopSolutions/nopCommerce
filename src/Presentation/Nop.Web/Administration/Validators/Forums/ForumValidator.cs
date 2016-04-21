using FluentValidation;
using Nop.Admin.Models.Forums;
using Nop.Core.Domain.Forums;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Forums
{
    public partial class ForumValidator : BaseNopValidator<ForumModel>
    {
        public ForumValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Fields.Name.Required"));
            RuleFor(x => x.ForumGroupId).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Forums.Forum.Fields.ForumGroupId.Required"));

            SetStringPropertiesMaxLength<Forum>(dbContext);
        }
    }
}