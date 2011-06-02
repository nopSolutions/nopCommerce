using FluentValidation;
using Nop.Admin.Models;
using Nop.Admin.Models.Affiliates;
using Nop.Core.Domain.Orders;
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