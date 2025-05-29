using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.CloudflareImages.Models;

/// <summary>
/// Represents configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Nop.Plugin.Misc.CloudflareImages.Enabled")]
    public bool Enabled { get; set; } = false;

    [NopResourceDisplayName("Nop.Plugin.Misc.CloudflareImages.RequestTimeout")]
    public int? RequestTimeout { get; set; } = CloudflareImagesDefaults.RequestTimeout;

    [NopResourceDisplayName("Nop.Plugin.Misc.CloudflareImages.AccountId")]
    public string AccountId { get; set; } = string.Empty;

    [NopResourceDisplayName("Nop.Plugin.Misc.CloudflareImages.AccessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [NopResourceDisplayName("Nop.Plugin.Misc.CloudflareImages.DeliveryUrl")]
    public string DeliveryUrl { get; set; } = string.Empty;
}