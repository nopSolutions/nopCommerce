using FluentValidation;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Topics
{
    public partial class TopicValidator : BaseNopValidator<TopicModel>
    {
        public TopicValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.ForumTopicLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.ForumTopicLength));

            SetDatabaseValidationRules<Topic>(dbContext);
        }
    }
}
