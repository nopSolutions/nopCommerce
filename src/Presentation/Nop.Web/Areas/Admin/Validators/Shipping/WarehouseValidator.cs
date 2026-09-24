using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class WarehouseValidator : BaseNopValidator<WarehouseModel>
{
    public WarehouseValidator(AddressSettings addressSettings, CustomerSettings customerSettings, ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.Warehouses.Fields.Name.Required"));
        RuleFor(model => model.Address).SetValidator(new AddressValidator(addressSettings, customerSettings, localizationService));
        
        SetDatabaseValidationRules<Warehouse>();
    }
}