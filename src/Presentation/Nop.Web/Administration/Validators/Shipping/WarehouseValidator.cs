using FluentValidation;
using Nop.Admin.Models.Shipping;
using Nop.Services.Localization;

namespace Nop.Admin.Validators.Shipping
{
    public class WarehouseValidator : AbstractValidator<WarehouseModel>
    {
        public WarehouseValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Fields.Name.Required"));
        }
    }
}