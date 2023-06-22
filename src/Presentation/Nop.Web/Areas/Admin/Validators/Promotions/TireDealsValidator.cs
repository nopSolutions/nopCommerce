using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.TireDeals;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Promotions
{
    public partial class TireDealsValidator : BaseNopValidator<TireDealModel>
    {
        public TireDealsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty");
            RuleFor(x => x.ShortDescription).NotEmpty().WithMessage("Short Description cannot be empty");
            RuleFor(x => x.LongDescription).NotEmpty().WithMessage("Long Description cannot be empty");
        }
    }
}