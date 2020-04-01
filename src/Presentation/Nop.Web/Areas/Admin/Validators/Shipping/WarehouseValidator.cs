using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping
{
    public partial class WarehouseValidator : BaseNopValidator<WarehouseModel>
    {
        public WarehouseValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Fields.Name.Required"));

            SetDatabaseValidationRules<Warehouse>(dataProvider);
        }
    }
}