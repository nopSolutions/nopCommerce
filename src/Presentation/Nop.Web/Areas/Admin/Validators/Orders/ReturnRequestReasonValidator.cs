using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Orders
{
    public partial class ReturnRequestReasonValidator : BaseNopValidator<ReturnRequestReasonModel>
    {
        public ReturnRequestReasonValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name.Required"));

            SetDatabaseValidationRules<ReturnRequestReason>(mappingEntityAccessor);
        }
    }
}