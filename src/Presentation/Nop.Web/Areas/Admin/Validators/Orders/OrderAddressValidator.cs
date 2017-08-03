using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Orders
{
    public partial class OrderAddressValidator : BaseNopValidator<OrderAddressModel>
    {
        public OrderAddressValidator()
        {
            var addressValidator = (AddressValidator) EngineContext.Current.ResolveUnregistered(typeof(AddressValidator));
            RuleFor(x => x.Address).SetValidator(addressValidator);
        }
    }
}