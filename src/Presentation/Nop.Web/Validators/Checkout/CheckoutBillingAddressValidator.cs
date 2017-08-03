using Nop.Core.Infrastructure;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Checkout;
using Nop.Web.Validators.Common;

namespace Nop.Web.Validators.Checkout
{
    public partial class CheckoutBillingAddressValidator : BaseNopValidator<CheckoutBillingAddressModel>
    {
        public CheckoutBillingAddressValidator()
        {
            var addressValidator = (AddressValidator)EngineContext.Current.ResolveUnregistered(typeof(AddressValidator));
            RuleFor(x => x.BillingNewAddress).SetValidator(addressValidator);
        }
    }
}