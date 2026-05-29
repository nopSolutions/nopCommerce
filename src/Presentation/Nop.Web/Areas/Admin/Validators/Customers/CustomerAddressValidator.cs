using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Validators.Common;

namespace Nop.Web.Areas.Admin.Validators.Customers;

public partial class CustomerAddressValidator : AbstractValidator<CustomerAddressModel>
{
    public CustomerAddressValidator(ILocalizationService localizationService,
        AddressSettings addressSettings,
        CustomerSettings customerSettings)
    {
        RuleFor(model => model.Address).SetValidator(new AddressValidator(addressSettings, customerSettings, localizationService));
    }
}