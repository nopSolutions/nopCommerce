using Nop.Core;

namespace Nop.Plugin.Misc.CloudflareImages.Domain;

/// <summary>
/// Represents a cloudflare images
/// </summary>
public class CloudflareImages : BaseEntity
{
    public string ThumbFileName { get; set; }

    public string ImageId { get; set; }
}