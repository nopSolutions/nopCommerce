using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Affiliate;

namespace Nop.Web.Validators.Affiliate;

public partial class ApplyAffiliateValidator : BaseNopValidator<ApplyAffiliateModel>
{
    public ApplyAffiliateValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.FriendlyUrlName).NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("Affiliate.ApplyAccount.FriendlyUrlName.Required"));
    }
}