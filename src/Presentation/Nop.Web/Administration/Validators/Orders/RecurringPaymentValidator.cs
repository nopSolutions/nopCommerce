using FluentValidation;
using Nop.Admin.Models.Orders;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Orders
{
    public class RecurringPaymentValidator : AbstractValidator<RecurringPaymentModel>
    {
        public RecurringPaymentValidator(ILocalizationService localizationService)
        {
        }
    }
}