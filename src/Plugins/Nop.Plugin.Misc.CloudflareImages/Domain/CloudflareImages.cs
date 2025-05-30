using Nop.Core;

namespace Nop.Plugin.Misc.CloudflareImages.Domain;

/// <summary>
/// Represents a Cloudflare Images
/// </summary>
public class CloudflareImages : BaseEntity
{
    /// <summary>
    /// Gets or sets the thumb file name
    /// </summary>
    public string ThumbFileName { get; set; }

    /// <summary>
    /// Gets or sets the cloudflare image identifier
    /// </summary>
    public string ImageId { get; set; }
}