using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Affiliate;

public record ApplyAffiliateModel : BaseNopModel
{
    public ApplyAffiliateModel()
    {
        ExistingAddresses = new List<AddressModel>();
    }

    public bool DisableFormInput { get; set; }
    public string Result { get; set; }
    public bool DisplayCaptcha { get; set; }

    [NopResourceDisplayName("Affiliate.ApplyAccount.FriendlyUrlName")]
    public string FriendlyUrlName { get; set; }

    [NopResourceDisplayName("Affiliate.ApplyAccount.ExistingAddresses")]
    public IList<AddressModel> ExistingAddresses { get; set; }
}