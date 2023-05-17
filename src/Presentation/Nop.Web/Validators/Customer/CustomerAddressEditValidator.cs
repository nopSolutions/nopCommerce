using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Common;

namespace Nop.Web.Validators.Customer
{
    public partial class CustomerAddressEditValidator : AbstractValidator<CustomerAddressEditModel>
    {
        public CustomerAddressEditValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            AddressSettings addressSettings,
            CustomerSettings customerSettings)
        {
            RuleFor(model => model.Address).SetValidator(new AddressValidator(localizationService, stateProvinceService, addressSettings, customerSettings));
        }
    }
}
