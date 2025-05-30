using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Models.Admin;

/// <summary>
/// Represents the onboarding callback model
/// </summary>
public record OnboardingCallbackModel : BaseNopModel
{
    #region Properties

    public int StoreId { get; set; }

    public string MerchantIdInPayPal { get; set; }

    public bool? PermissionsGranted { get; set; }

    public string AccountStatus { get; set; }

    public string RiskStatus { get; set; }

    public bool? ConsentStatus { get; set; }

    public string ProductIntentID { get; set; }

    public bool? IsEmailConfirmed { get; set; }

    public string ReturnMessage { get; set; }

    #endregion
}