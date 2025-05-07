using Nop.Core;

namespace Nop.Plugin.Misc.CloudflareImages.Domain;

/// <summary>
/// Represents a Cloudflare Images
/// </summary>
public class CloudflareImages : BaseEntity
{
    public string ThumbFileName { get; set; }

    public string ImageId { get; set; }
}