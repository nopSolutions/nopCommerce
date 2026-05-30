using FluentValidation;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping;

public partial class DeliveryDateValidator : BaseNopValidator<DeliveryDateModel>
{
    public DeliveryDateValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Fields.Name.Required"));

        SetDatabaseValidationRules<DeliveryDate>();
    }
}