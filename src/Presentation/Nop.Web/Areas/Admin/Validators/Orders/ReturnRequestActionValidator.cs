using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class ReturnRequestActionValidator : BaseNopValidator<ReturnRequestActionModel>
{
    public ReturnRequestActionValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Name.Required"));

        SetDatabaseValidationRules<ReturnRequestAction>();
    }
}