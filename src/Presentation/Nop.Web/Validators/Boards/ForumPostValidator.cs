using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.Boards;

namespace Nop.Web.Validators.Boards
{
    public class ForumPostValidator : AbstractValidator<ForumPostModel>
    {
        public ForumPostValidator(ILocalizationService localizationService)
        {            
            RuleFor(x => x.Text).NotEmpty().WithMessage(localizationService.GetResource("Forum.TextCannotBeEmpty"));
        }
    }
}