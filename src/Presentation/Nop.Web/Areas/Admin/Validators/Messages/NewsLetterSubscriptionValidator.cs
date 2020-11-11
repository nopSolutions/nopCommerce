using FluentValidation;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Messages
{
    public partial class NewsLetterSubscriptionValidator : BaseNopValidator<NewsletterSubscriptionModel>
    {
        public NewsLetterSubscriptionValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Promotions.NewsLetterSubscriptions.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));

            SetDatabaseValidationRules<NewsLetterSubscription>(dataProvider);
        }
    }
}