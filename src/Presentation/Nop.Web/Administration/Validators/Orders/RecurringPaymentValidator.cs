using FluentValidation;
using Nop.Admin.Models.Orders;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Orders
{
    public class RecurringPaymentValidator : BaseNopValidator<RecurringPaymentModel>
    {
        public RecurringPaymentValidator(ILocalizationService localizationService)
        {
        }
    }
}