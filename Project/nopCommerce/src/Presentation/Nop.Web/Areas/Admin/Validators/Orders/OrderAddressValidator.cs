using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Validators.Common;

namespace Nop.Web.Areas.Admin.Validators.Orders;

public partial class OrderAddressValidator : AbstractValidator<OrderAddressModel>
{
    public OrderAddressValidator(ILocalizationService localizationService,
        AddressSettings addressSettings)
    {
        RuleFor(model => model.Address).SetValidator(new AddressValidator(addressSettings, localizationService));
    }
}