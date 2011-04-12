using FluentValidation;
using Nop.Admin.Models;
using Nop.Services.Localization;

namespace Nop.Admin.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerModel>
    {
        public CustomerValidator(ILocalizationService localizationService)
        {
            //UNDONE add requred validation (use 'FormFieldSettings' class)
        }
    }
}