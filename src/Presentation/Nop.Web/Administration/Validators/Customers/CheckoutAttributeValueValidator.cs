using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Customers
{
    public class CustomerAttributeValueValidator : AbstractValidator<CustomerAttributeValueModel>
    {
        public CustomerAttributeValueValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerAttributes.Values.Fields.Name.Required"));
        }
    }
}