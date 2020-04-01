using FluentValidation;
using Nop.Core.Domain.Discounts;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Discounts
{
    public partial class DiscountValidator : BaseNopValidator<DiscountModel>
    {
        public DiscountValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.Name.Required"));

            SetDatabaseValidationRules<Discount>(dataProvider);
        }
    }
}