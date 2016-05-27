using FluentValidation;
using Nop.Admin.Models.Topics;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Topics
{
    public partial class TopicValidator : BaseNopValidator<TopicModel>
    {
        public TopicValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SystemName).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Topics.Fields.SystemName.Required"));
        }
    }
}