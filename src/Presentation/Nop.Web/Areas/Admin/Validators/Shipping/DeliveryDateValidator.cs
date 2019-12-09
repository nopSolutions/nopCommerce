using FluentValidation;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping
{
    public partial class DeliveryDateValidator : BaseNopValidator<DeliveryDateModel>
    {
        public DeliveryDateValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.DeliveryDates.Fields.Name.Required"));

            SetDatabaseValidationRules<DeliveryDate>(dataProvider);
        }
    }
}