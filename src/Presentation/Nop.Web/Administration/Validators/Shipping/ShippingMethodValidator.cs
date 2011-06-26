using FluentValidation;
using Nop.Admin.Models.Shipping;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Shipping
{
    public class ShippingMethodValidator : AbstractValidator<ShippingMethodModel>
    {
        public ShippingMethodValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.Methods.Fields.Name.Required"));
        }
    }
}