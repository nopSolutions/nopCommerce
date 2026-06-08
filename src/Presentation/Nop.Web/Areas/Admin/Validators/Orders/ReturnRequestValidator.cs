using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class ReturnRequestValidator : BaseNopValidator<ReturnRequestModel>
{
    public ReturnRequestValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.ReasonForReturn).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReasonForReturn.Required"));
        RuleFor(x => x.RequestedAction).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.RequestedAction.Required"));
        RuleFor(x => x.Quantity).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.Quantity.Required"));
        RuleFor(x => x.Quantity).Must((model, value) => value >= model.ReturnedQuantity).WithMessageAwait(async model => string.Format(await localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.Quantity.MustBeEqualOrGreaterThanReturnedQuantityField"), model.ReturnedQuantity));
        RuleFor(x => x.ReturnedQuantity).GreaterThan(-1).WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeEqualOrGreaterThanZero"));
        RuleFor(x => x.ReturnedQuantity).Must((model, value) => value <= model.Quantity).WithMessageAwait(async model => string.Format(await localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeLessOrEqualQuantityField"), model.Quantity));

        SetDatabaseValidationRules<ReturnRequest>();
    }
}