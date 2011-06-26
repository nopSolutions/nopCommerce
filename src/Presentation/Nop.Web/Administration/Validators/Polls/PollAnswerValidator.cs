using FluentValidation;
using Nop.Admin.Models.Polls;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Polls
{
    public class PollAnswerValidator : AbstractValidator<PollAnswerModel>
    {
        public PollAnswerValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Polls.Answers.Fields.Name.Required"));
        }
    }
}