using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.TireDeals;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Validators.Promotions
{
    public partial class TireDealsSearchValidator : BaseNopValidator<TireDealSearchModel>
    {
        public TireDealsSearchValidator(ILocalizationService localizationService)
        {
            
        }
    }
}