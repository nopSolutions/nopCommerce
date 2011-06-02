using FluentValidation;
using Nop.Admin.Models.Orders;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Orders
{
    public class CheckoutAttributeValidator : AbstractValidator<CheckoutAttributeModel>
    {
        public CheckoutAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name.Required"));
        }
    }
}