using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Paystack.Models;

/// <summary>
/// Represents the Paystack plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Paystack.Fields.SecretKey")]
    public string SecretKey { get; set; } = string.Empty;
    public bool SecretKey_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Paystack.Fields.PublicKey")]
    public string PublicKey { get; set; } = string.Empty;
    public bool PublicKey_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Paystack.Fields.CallbackUrl")]
    public string CallbackUrl { get; set; } = string.Empty;
    public bool CallbackUrl_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Paystack.Fields.WebhookSecret")]
    public string WebhookSecret { get; set; } = string.Empty;
    public bool WebhookSecret_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Paystack.Fields.AdditionalFee")]
    public decimal AdditionalFee { get; set; }
    public bool AdditionalFee_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Payments.Paystack.Fields.AdditionalFeePercentage")]
    public bool AdditionalFeePercentage { get; set; }
    public bool AdditionalFeePercentage_OverrideForStore { get; set; }
}
