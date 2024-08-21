using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.AmazonPay.Models;

public record OnboardingModel : BaseNopModel
{
    #region Properties

    public bool SetCredentialsManually { get; set; }

    public int Region { get; set; }
    public bool Region_OverrideForStore { get; set; }

    #endregion
}