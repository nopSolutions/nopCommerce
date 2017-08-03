using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Areas.Admin.Validators.Common;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Affiliates
{
    public partial class AffiliateValidator : BaseNopValidator<AffiliateModel>
    {
        public AffiliateValidator()
        {
            var addressValidator = (AddressValidator) EngineContext.Current.ResolveUnregistered(typeof(AddressValidator));
            RuleFor(x => x.Address).SetValidator(addressValidator);
        }
    }
}