using Nop.Core.Infrastructure;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Nop.Web.Validators.Common;

namespace Nop.Web.Validators.Customer
{
    public partial class CustomerAddressEditValidator : BaseNopValidator<CustomerAddressEditModel>
    {
        public CustomerAddressEditValidator()
        {
            var addressValidator = (AddressValidator)EngineContext.Current.ResolveUnregistered(typeof(AddressValidator));
            RuleFor(x => x.Address).SetValidator(addressValidator);
        }
    }
}