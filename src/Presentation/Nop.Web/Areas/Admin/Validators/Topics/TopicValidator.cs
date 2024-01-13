using FluentValidation;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Topics;

public partial class TopicValidator : BaseNopValidator<TopicModel>
{
    public TopicValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.SeName)
            .Length(0, NopSeoDefaults.ForumTopicLength)
            .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.ForumTopicLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => x.IsPasswordProtected)
            .WithMessageAwait(localizationService.GetResourceAsync("Validation.Password.IsNotEmpty"));

        SetDatabaseValidationRules<Topic>();
    }
}