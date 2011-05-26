using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.Newsletter;

namespace Nop.Web.Validators.Newsletter
{
    public class NewsletterBoxValidator : AbstractValidator<NewsletterBoxModel>
    {
        public NewsletterBoxValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("*");
            RuleFor(x => x.Email).EmailAddress().WithMessage("*");
        }}
}