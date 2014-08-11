using FluentValidation;
using Nop.Admin.Models.Affiliates;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Affiliates
{
    public class AffiliateValidator : BaseNopValidator<AffiliateModel>
    {
        public AffiliateValidator(ILocalizationService localizationService)
        {
        }
    }
}