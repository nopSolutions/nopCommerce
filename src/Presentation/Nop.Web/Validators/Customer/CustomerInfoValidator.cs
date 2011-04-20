using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public class CustomerInfoValidator : AbstractValidator<CustomerInfoModel>
    {
        public CustomerInfoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress(); //TODO locale email not valid message
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.FirstName.Required"));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.LastName.Required"));
        }}
}