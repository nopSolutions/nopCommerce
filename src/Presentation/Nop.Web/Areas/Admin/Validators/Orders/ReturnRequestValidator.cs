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

            SetDatabaseValidationRules<ReturnRequest>(mappingEntityAccessor);
        }
    }
}