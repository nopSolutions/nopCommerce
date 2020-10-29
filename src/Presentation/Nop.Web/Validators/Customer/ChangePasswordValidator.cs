using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public partial class ChangePasswordValidator : BaseNopValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.ChangePassword.Fields.OldPassword.Required").Result);
            RuleFor(x => x.NewPassword).IsPassword(localizationService, customerSettings);
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(localizationService.GetResourceAsync("Account.ChangePassword.Fields.ConfirmNewPassword.Required").Result);
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(localizationService.GetResourceAsync("Account.ChangePassword.Fields.NewPassword.EnteredPasswordsDoNotMatch").Result);            
        }
    }
}