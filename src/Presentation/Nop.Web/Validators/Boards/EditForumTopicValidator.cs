using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.Boards;

namespace Nop.Web.Validators.Boards
{
    public class EditForumTopicValidator : AbstractValidator<EditForumTopicModel>
    {
        public EditForumTopicValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Forum.TopicSubjectCannotBeEmpty"));
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}