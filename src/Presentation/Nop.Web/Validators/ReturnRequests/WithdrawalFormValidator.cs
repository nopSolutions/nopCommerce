using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Order;

namespace Nop.Web.Validators.ReturnRequests;

public partial class WithdrawalFormValidator : BaseNopValidator<WithdrawalFormModel>
{
    public WithdrawalFormValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
    {
        RuleFor(x => x.OrderNumber)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("ReturnRequests.WithdrawalForm.OrderNumber.Required"));

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("ReturnRequests.WithdrawalForm.EmailAddress.Required"));
        RuleFor(x => x.EmailAddress)
            .IsEmailAddress()
            .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
    }
}