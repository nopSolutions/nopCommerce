using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Orders
{
    public partial class ReturnRequestValidator : BaseNopValidator<ReturnRequestModel>
    {
        public ReturnRequestValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.ReasonForReturn).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReasonForReturn.Required"));
            RuleFor(x => x.RequestedAction).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.RequestedAction.Required"));
            RuleFor(x => x.Quantity).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.Quantity.Required"));
            RuleFor(x => x.Quantity).Must((model, value) => value >= model.ReturnedQuantity).WithMessage(model => string.Format(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.Quantity.MustBeEqualOrGreaterThanReturnedQuantityField").Result, model.ReturnedQuantity));
            RuleFor(x => x.ReturnedQuantity).GreaterThan(-1).WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeEqualOrGreaterThanZero"));
            RuleFor(x => x.ReturnedQuantity).Must((model, value) => value <= model.Quantity).WithMessage(model => string.Format(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeLessOrEqualQuantityField").Result, model.Quantity));

            SetDatabaseValidationRules<ReturnRequest>(mappingEntityAccessor);
        }
    }
}