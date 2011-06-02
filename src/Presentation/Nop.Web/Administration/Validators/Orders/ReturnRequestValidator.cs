using FluentValidation;
using Nop.Admin.Models.Orders;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Orders
{
    public class ReturnRequestValidator : AbstractValidator<ReturnRequestModel>
    {
        public ReturnRequestValidator(ILocalizationService localizationService)
        {
        }
    }
}