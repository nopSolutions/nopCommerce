using Nop.Core.Infrastructure;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Checkout;
using Nop.Web.Validators.Common;

namespace Nop.Web.Validators.Checkout
{
    public partial class CheckoutShippingAddressValidator : BaseNopValidator<CheckoutShippingAddressModel>
    {
        public CheckoutShippingAddressValidator()
        {
            var addressValidator = (AddressValidator)EngineContext.Current.ResolveUnregistered(typeof(AddressValidator));
            RuleFor(x => x.ShippingNewAddress).SetValidator(addressValidator);
        }
    }
}