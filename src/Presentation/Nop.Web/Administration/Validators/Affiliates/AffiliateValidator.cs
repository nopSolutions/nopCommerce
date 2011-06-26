using FluentValidation;
using Nop.Admin.Models.Affiliates;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Affiliates
{
    public class AffiliateValidator : AbstractValidator<AffiliateModel>
    {
        public AffiliateValidator(ILocalizationService localizationService)
        {
        }
    }
}