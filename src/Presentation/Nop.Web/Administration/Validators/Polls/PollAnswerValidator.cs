using FluentValidation;
using Nop.Admin.Models.Polls;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Polls
{
    public partial class PollAnswerValidator : BaseNopValidator<PollAnswerModel>
    {
        public PollAnswerValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Polls.Answers.Fields.Name.Required"));
        }
    }
}