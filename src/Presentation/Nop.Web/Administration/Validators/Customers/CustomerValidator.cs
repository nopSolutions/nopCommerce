using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Customers
{
    public class CustomerValidator : AbstractValidator<CustomerModel>
    {
        public CustomerValidator(ILocalizationService localizationService)
        {
            //we store 'UsernamesEnabled' and 'AllowUsersToChangeUsernames' as hidden fields; otherwise, they always be false.
            //RuleFor(x => x.Username)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Username.Required"))
            //    .When(x => x.UsernamesEnabled && x.AllowUsersToChangeUsernames);

            //RuleFor(x => x.Email)
            //    .NotNull()
            //    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Email.Required"));

            //RuleFor(x => x.Email)
            //    .EmailAddress()
            //    .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
        }
    }
}