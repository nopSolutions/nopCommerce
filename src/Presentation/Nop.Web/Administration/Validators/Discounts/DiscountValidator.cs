using FluentValidation;
using Nop.Admin.Models.Discounts;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Discounts
{
    public partial class DiscountValidator : BaseNopValidator<DiscountModel>
    {
        public DiscountValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.Name.Required"));

            SetStringPropertiesMaxLength<Discount>(dbContext);
        }
    }
}