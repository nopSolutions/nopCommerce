using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class NewsLetterSubscriptionValidator : BaseNopValidator<NewsLetterSubscriptionModel>
{
    public NewsLetterSubscriptionValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Admin.Promotions.NewsLetterSubscription.Fields.Email.Required");
        RuleFor(x => x.Email)
            .IsEmailAddress()
            .WithMessage("Admin.Common.WrongEmail");

        SetDatabaseValidationRules<NewsLetterSubscription>();
    }
}