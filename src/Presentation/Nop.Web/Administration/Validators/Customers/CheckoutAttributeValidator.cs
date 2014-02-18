using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Customers
{
    public class CustomerAttributeValidator : AbstractValidator<CustomerAttributeModel>
    {
        public CustomerAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.Name.Required"));
        }
    }
}