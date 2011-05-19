using FluentValidation;
using Nop.Admin.Models;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class ReturnRequestValidator : AbstractValidator<ReturnRequestModel>
    {
        public ReturnRequestValidator(ILocalizationService localizationService)
        {
        }
    }
}