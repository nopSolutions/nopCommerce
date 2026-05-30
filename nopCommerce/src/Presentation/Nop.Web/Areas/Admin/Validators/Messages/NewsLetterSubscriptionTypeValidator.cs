using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages;

public partial class NewsLetterSubscriptionTypeValidator : BaseNopValidator<NewsLetterSubscriptionTypeModel>
{
    public NewsLetterSubscriptionTypeValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptionType.Fields.Name.Required"));

        SetDatabaseValidationRules<NewsLetterSubscriptionType>();
    }
}