using FluentValidation;
using Nop.Admin.Models.Polls;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Polls
{
    public class PollValidator : AbstractValidator<PollModel>
    {
        public PollValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Polls.Fields.Name.Required"));
        }
    }
}